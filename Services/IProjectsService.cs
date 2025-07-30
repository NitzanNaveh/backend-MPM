using ProjectManager.Api.Dtos.Projects;

namespace ProjectManager.Api.Services;

public interface IProjectsService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllProjectsAsync(int userId);
    Task<ProjectResponseDto?> GetProjectByIdAsync(int projectId, int userId);
    Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int userId);
    Task<ProjectResponseDto> UpdateProjectAsync(int projectId, CreateProjectDto updateProjectDto, int userId);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
} 