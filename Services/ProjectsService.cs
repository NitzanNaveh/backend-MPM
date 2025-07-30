using Microsoft.EntityFrameworkCore;
using ProjectManager.Api.Data;
using ProjectManager.Api.Dtos.Projects;
using ProjectManager.Api.Entities;

namespace ProjectManager.Api.Services;

public class ProjectsService : IProjectsService
{
    private readonly AppDbContext _context;

    public ProjectsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetAllProjectsAsync(int userId)
    {
        var projects = await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Where(p => p.OwnerId == userId)
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                OwnerId = p.OwnerId,
                OwnerName = $"{p.Owner.FirstName} {p.Owner.LastName}",
                CreatedAt = p.CreatedAt,
                TaskCount = p.Tasks.Count
            })
            .ToListAsync();

        return projects;
    }

    public async Task<ProjectResponseDto?> GetProjectByIdAsync(int projectId, int userId)
    {
        var project = await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

        if (project == null)
            return null;

        return new ProjectResponseDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName = $"{project.Owner.FirstName} {project.Owner.LastName}",
            CreatedAt = project.CreatedAt,
            TaskCount = project.Tasks.Count
        };
    }

    public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto createProjectDto, int userId)
    {
        var project = new Project
        {
            Title = createProjectDto.Title,
            Description = createProjectDto.Description,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);
        
        return new ProjectResponseDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName = $"{user!.FirstName} {user.LastName}",
            CreatedAt = project.CreatedAt,
            TaskCount = 0
        };
    }

    public async Task<ProjectResponseDto> UpdateProjectAsync(int projectId, CreateProjectDto updateProjectDto, int userId)
    {
        var project = await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

        if (project == null)
            throw new InvalidOperationException("Project not found or access denied");

        project.Title = updateProjectDto.Title;
        project.Description = updateProjectDto.Description;

        await _context.SaveChangesAsync();

        return new ProjectResponseDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName = $"{project.Owner.FirstName} {project.Owner.LastName}",
            CreatedAt = project.CreatedAt,
            TaskCount = project.Tasks.Count
        };
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
} 