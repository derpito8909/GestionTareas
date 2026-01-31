import { Component, DestroyRef, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
  FormArray,
  FormControl,
  FormGroup
 } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { TasksFacade } from '../../data-access/task.facade';
import { UsersFacade } from '../../../users/data-access/users.facade';
import { ApiErrorBannerComponent } from '../../../../shared/ui/api-error-banner.component';

import { TaskAdditionalInfo, Prioridad } from '../../domain/task-additional-info.model';

type MetaRowForm = FormGroup<{
  key: FormControl<string>;
  value: FormControl<string>;
}>;
@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ApiErrorBannerComponent],
  templateUrl: './task-create.component.html',
})
export class TaskCreateComponent {
  readonly tasksFacade = inject(TasksFacade);
  readonly usersFacade = inject(UsersFacade);

  readonly metaInfo = signal<string | null>(null);


  private readonly fb = inject(NonNullableFormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  readonly additionalPreview = signal<string>('');

  readonly tagInput = this.fb.control('');
  readonly metaKeyInput = this.fb.control('');
  readonly metaValueInput = this.fb.control('');

  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    assignedUserId: ['', [Validators.required]],
    prioridad: ['Baja' as Prioridad, [Validators.required]],
    fechaEstimada: [''],
    etiquetas: this.fb.array<FormControl<string>>([]),
    meta: this.fb.array<MetaRowForm>([]),
  });

  get etiquetas(): FormArray<FormControl<string>> {
    return this.form.controls.etiquetas;
  }

  get meta(): FormArray<MetaRowForm> {
    return this.form.controls.meta;
  }

  ngOnInit() {
    if (this.usersFacade.users().length === 0) {
      this.usersFacade.load();
    }
    this.updatePreview();
    this.form.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.updatePreview());
  }
  addTag() {
    const value = (this.tagInput.value ?? '').trim();
    if (!value) return;
    const exists = this.etiquetas.controls.some(c => c.value === value);
    if (!exists) this.etiquetas.push(this.fb.control(value));

    this.tagInput.setValue('');
    this.updatePreview();
  }

  removeTag(index: number) {
    this.etiquetas.removeAt(index);
    this.updatePreview();
  }
  addMeta() {
    const key = (this.metaKeyInput.value ?? '').trim();
    const value = (this.metaValueInput.value ?? '').trim();

    this.metaInfo.set(null);

    if (!key || !value) return;

    const keyNormalized = key.toLowerCase();

    const existingIndex = this.meta.controls.findIndex(r =>
      r.controls.key.value.trim().toLowerCase() === keyNormalized
    );

    if (existingIndex >= 0) {
      this.meta.at(existingIndex).controls.value.setValue(value);

      this.metaInfo.set(`Meta "${key}" actualizada.`);
    } else {
      const row: MetaRowForm = this.fb.group({
        key: this.fb.control(key),
        value: this.fb.control(value),
      });

      this.meta.push(row);

      this.metaInfo.set(`Meta "${key}" agregada.`);
    }

    this.metaKeyInput.setValue('');
    this.metaValueInput.setValue('');
    this.updatePreview();
  }

  removeMeta(index: number) {
    this.meta.removeAt(index);
    this.updatePreview();
  }

  private buildAdditionalInfo(): TaskAdditionalInfo {
    const prioridad = this.form.controls.prioridad.value;
    const fechaEstimada = (this.form.controls.fechaEstimada.value ?? '').trim();

    const etiquetas = this.etiquetas.controls
      .map(c => (c.value ?? '').trim())
      .filter(Boolean);

    const metaObj: Record<string, string> = {};
    for (const row of this.meta.controls) {
      const k = (row.controls.key.value ?? '').trim();
      const v = (row.controls.value.value ?? '').trim();
      if (!k || !v) continue;
      metaObj[k] = v;
    }

    const info: TaskAdditionalInfo = { Prioridad: prioridad };

    if (fechaEstimada) info.FechaEstimada = fechaEstimada;
    if (etiquetas.length > 0) info.Etiquetas = etiquetas;
    if (Object.keys(metaObj).length > 0) info.Meta = metaObj;

    return info;
  }

  private buildAdditionalInfoJson(): string {
    return JSON.stringify(this.buildAdditionalInfo());
  }

  private updatePreview() {
    this.additionalPreview.set(this.buildAdditionalInfoJson());
  }

  create() {
    const v = this.form.getRawValue();

    this.tasksFacade.create(
      {
        title: v.title,
        description: v.description || null,
        assignedUserId: Number(v.assignedUserId),
        additionalInfoJson: this.buildAdditionalInfoJson(),
      },
      () => {
        this.form.reset({
          title: '',
          description: '',
          assignedUserId: '',
          prioridad: 'Baja',
          fechaEstimada: '',
        });
        while (this.etiquetas.length) this.etiquetas.removeAt(0);
        while (this.meta.length) this.meta.removeAt(0);

        this.tagInput.setValue('');
        this.metaKeyInput.setValue('');
        this.metaValueInput.setValue('');
        this.metaInfo.set(null);

        this.updatePreview();
      }
    );
  }
}
