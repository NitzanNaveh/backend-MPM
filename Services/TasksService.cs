using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Data;
using ProjectManager.Api.Dtos.Tasks;
using ProjectManager.Api.Entities;

namespace ProjectManager.Api.Services;

public class TasksService : ITasksService
{
    private readonly AppDbContext _context;

    public TasksService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksByProjectAsync(int projectId, int userId)
    {
        Console.WriteLine($"TasksService: GetTasksByProjectAsync called with ProjectId={projectId}, UserId={userId}");
        
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Where(t => t.ProjectId == projectId && t.Project.OwnerId == userId)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                ProjectId = t.ProjectId,
                ProjectTitle = t.Project.Title,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        Console.WriteLine($"TasksService: Found {tasks.Count()} tasks for project {projectId}");
        foreach (var task in tasks)
        {
            Console.WriteLine($"TasksService: Task {task.Id}: {task.Title}");
        }

        return tasks;
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.OwnerId == userId);

        if (task == null)
            return null;

        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            ProjectId = task.ProjectId,
            ProjectTitle = task.Project.Title,
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId)
    {
        Console.WriteLine($"TasksService: CreateTaskAsync called with ProjectId={createTaskDto.ProjectId}, Title='{createTaskDto.Title}', UserId={userId}");
        
        // Verify project exists and user owns it
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == createTaskDto.ProjectId && p.OwnerId == userId);

        if (project == null)
        {
            Console.WriteLine($"TasksService: Project not found or access denied. ProjectId={createTaskDto.ProjectId}, UserId={userId}");
            throw new InvalidOperationException("Project not found or access denied");
        }

        Console.WriteLine($"TasksService: Project found: {project.Title}");

        var task = new TaskItem
        {
            Title = createTaskDto.Title,
            DueDate = createTaskDto.DueDate,
            ProjectId = createTaskDto.ProjectId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        Console.WriteLine($"TasksService: Creating task: {task.Title} for project {task.ProjectId}");

        _context.Tasks.Add(task);
        
        Console.WriteLine($"TasksService: Task added to context, saving changes...");
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"TasksService: Task saved successfully with ID: {task.Id}");

        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            ProjectId = task.ProjectId,
            ProjectTitle = project.Title,
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, int userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.OwnerId == userId);

        if (task == null)
            throw new InvalidOperationException("Task not found or access denied");

        task.Title = updateTaskDto.Title;
        task.DueDate = updateTaskDto.DueDate;
        task.IsCompleted = updateTaskDto.IsCompleted;

        await _context.SaveChangesAsync();

        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            ProjectId = task.ProjectId,
            ProjectTitle = task.Project.Title,
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<bool> DeleteTaskAsync(int taskId, int userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.OwnerId == userId);

        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
} 