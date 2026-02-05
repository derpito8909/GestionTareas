using System.Text;
using GestionTareas.Application.Dtos;
using GestionTareas.Application.Interfaces;
using GestionTareas.Domain.Entities;
using GestionTareas.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GestionTareas.Infrastructure.Repositories;

public sealed class TasksRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TasksRepository(AppDbContext db) => _db = db;

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct)
    {
        await _db.Tasks.AddAsync(task, ct);
        return task;
    }

    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct)
        => _db.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedUser) // ✅ para AssignedUserName
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<TaskItem>> ListAsync(TaskQuery query, CancellationToken ct)
    {
        if (!query.HasJsonFilters)
        {
            IQueryable<TaskItem> q = _db.Tasks
                .Include(t => t.AssignedUser)
                .AsNoTracking();

            if (query.UserId.HasValue)
                q = q.Where(t => t.AssignedUserId == query.UserId.Value);

            if (query.Status.HasValue)
                q = q.Where(t => t.Status == query.Status.Value);

            q = query.OrderByCreatedAtDesc
                ? q.OrderByDescending(t => t.CreatedAt)
                : q.OrderBy(t => t.CreatedAt);

            return await q.ToListAsync(ct);
        }
        
        var sql = new StringBuilder();
        var parameters = new List<SqlParameter>();

        sql.AppendLine("SELECT t.*");
        sql.AppendLine("FROM dbo.Tasks t");
        sql.AppendLine("WHERE 1=1");
        
        if (query.UserId.HasValue)
        {
            sql.AppendLine("AND t.AssignedUserId = @userId");
            parameters.Add(new SqlParameter("@userId", query.UserId.Value));
        }

        if (query.Status.HasValue)
        {
            sql.AppendLine("AND t.Status = @status");
            parameters.Add(new SqlParameter("@status", query.Status.Value.ToString()));
        }
        
        if (!string.IsNullOrWhiteSpace(query.Priority))
        {
            sql.AppendLine("AND JSON_VALUE(t.AdditionalInfoJson, '$.prioridad') = @priority");
            parameters.Add(new SqlParameter("@priority", query.Priority.Trim()));
        }
        
        if (query.DueDateFrom.HasValue)
        {
            sql.AppendLine("AND TRY_CONVERT(date, JSON_VALUE(t.AdditionalInfoJson, '$.fechaEstimada')) >= @dueFrom");
            parameters.Add(new SqlParameter("@dueFrom", query.DueDateFrom.Value.Date));
        }

        if (query.DueDateTo.HasValue)
        {
            sql.AppendLine("AND TRY_CONVERT(date, JSON_VALUE(t.AdditionalInfoJson, '$.fechaEstimada')) <= @dueTo");
            parameters.Add(new SqlParameter("@dueTo", query.DueDateTo.Value.Date));
        }
        
        if (!string.IsNullOrWhiteSpace(query.Tag))
        {
            sql.AppendLine(@"
            AND EXISTS (
                SELECT 1
                FROM OPENJSON(t.AdditionalInfoJson, '$.etiquetas') tags
                WHERE tags.[value] = @tag
            )");
            parameters.Add(new SqlParameter("@tag", query.Tag.Trim()));
        }

        sql.AppendLine(query.OrderByCreatedAtDesc
            ? "ORDER BY t.CreatedAt DESC"
            : "ORDER BY t.CreatedAt ASC");
        
        var result = await _db.Tasks
            .FromSqlRaw(sql.ToString(), parameters.ToArray())
            .Include(t => t.AssignedUser)
            .AsNoTracking()
            .ToListAsync(ct);

        return result;
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}