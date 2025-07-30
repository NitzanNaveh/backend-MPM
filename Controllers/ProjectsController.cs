using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Dtos.Projects;
using ProjectManager.Api.Services;
using System.Security.Claims;

namespace ProjectManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectsService _projectsService;

    public ProjectsController(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var userId = GetUserId();
        var projects = await _projectsService.GetAllProjectsAsync(userId);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var userId = GetUserId();
        var project = await _projectsService.GetProjectByIdAsync(id, userId);
        
        if (project == null)
            return NotFound();
            
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
    {
        try
        {
            Console.WriteLine($"CreateProject called with: Title='{createProjectDto.Title}', Description='{createProjectDto.Description}'");
            
            // Check if model validation failed
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                return BadRequest(new { message = string.Join(", ", errors) });
            }

            var userId = GetUserId();
            Console.WriteLine($"User ID from context: {userId}");
            
            var project = await _projectsService.CreateProjectAsync(createProjectDto, userId);
            Console.WriteLine($"Project created successfully with ID: {project.Id}");
            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"InvalidOperationException in CreateProject: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Project creation error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "An unexpected error occurred while creating the project." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] CreateProjectDto updateProjectDto)
    {
        try
        {
            var userId = GetUserId();
            var project = await _projectsService.UpdateProjectAsync(id, updateProjectDto, userId);
            return Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = GetUserId();
        var deleted = await _projectsService.DeleteProjectAsync(id, userId);
        
        if (!deleted)
            return NotFound();
            
        return NoContent();
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            return userId;
            
        throw new InvalidOperationException("User ID not found in context");
    }
} 