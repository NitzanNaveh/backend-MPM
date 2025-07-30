using ProjectManager.Api.Dtos.Tasks;

namespace ProjectManager.Api.Services;

public interface ITasksService
{
    Task<IEnumerable<TaskResponseDto>> GetTasksByProjectAsync(int projectId, int userId);
    Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId);
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);
    Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, int userId);
    Task<bool> DeleteTaskAsync(int taskId, int userId);
} 