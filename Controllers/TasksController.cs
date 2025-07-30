using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Dtos.Tasks;
using ProjectManager.Api.Services;
using System.Security.Claims;

namespace ProjectManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITasksService _tasksService;

    public TasksController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetTasksByProject(int projectId)
    {
        try
        {
            Console.WriteLine($"GetTasksByProject called for project {projectId}");
            var userId = GetUserId();
            Console.WriteLine($"User ID from context: {userId}");
            var tasks = await _tasksService.GetTasksByProjectAsync(projectId, userId);
            Console.WriteLine($"Retrieved {tasks.Count()} tasks for project {projectId}");
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetTasksByProject error: {ex.Message}");
            return BadRequest(new { message = "Failed to retrieve tasks for the project." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        try
        {
            Console.WriteLine($"GetTaskById called for task {id}");
            var userId = GetUserId();
            Console.WriteLine($"User ID from context: {userId}");
            var task = await _tasksService.GetTaskByIdAsync(id, userId);
            
            if (task == null)
            {
                Console.WriteLine($"Task {id} not found");
                return NotFound();
            }
            
            Console.WriteLine($"Retrieved task {id}");
            return Ok(task);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetTaskById error: {ex.Message}");
            return BadRequest(new { message = "Failed to retrieve the task." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        try
        {
            Console.WriteLine($"CreateTask called with: Title='{createTaskDto.Title}', ProjectId={createTaskDto.ProjectId}");
            
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
            
            var task = await _tasksService.CreateTaskAsync(createTaskDto, userId);
            Console.WriteLine($"Task created successfully with ID: {task.Id}");
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"InvalidOperationException in CreateTask: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Task creation error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "An unexpected error occurred while creating the task." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        try
        {
            Console.WriteLine($"UpdateTask called for ID {id}: Title='{updateTaskDto.Title}', IsCompleted={updateTaskDto.IsCompleted}");
            
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
            
            var task = await _tasksService.UpdateTaskAsync(id, updateTaskDto, userId);
            Console.WriteLine($"Task updated successfully");
            return Ok(task);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"InvalidOperationException in UpdateTask: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Task update error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return BadRequest(new { message = "An unexpected error occurred while updating the task." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            Console.WriteLine($"DeleteTask called for task {id}");
            var userId = GetUserId();
            Console.WriteLine($"User ID from context: {userId}");
            var deleted = await _tasksService.DeleteTaskAsync(id, userId);
            
            if (!deleted)
            {
                Console.WriteLine($"Task {id} not found or access denied");
                return NotFound();
            }
            
            Console.WriteLine($"Task {id} deleted successfully");
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteTask error: {ex.Message}");
            return BadRequest(new { message = "Failed to delete the task." });
        }
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            return userId;
            
        throw new InvalidOperationException("User ID not found in context");
    }
} 