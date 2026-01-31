export type Prioridad = 'Baja' | 'Media' | 'Alta';

export interface TaskAdditionalInfo {
  Prioridad: Prioridad;
  FechaEstimada?: string;
  Etiquetas?: string[];
  Meta?: Record<string, string>;
}
