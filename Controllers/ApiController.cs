using Microsoft.AspNetCore.Mvc;

namespace AKP.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        // Sample in-memory store for demonstration
        private static readonly List<TodoItem> SampleItems =
        [
            new TodoItem { Id = 1, Title = "Connect frontend to controller", IsCompleted = true },
            new TodoItem { Id = 2, Title = "Build custom frontend UI in wwwroot", IsCompleted = false },
            new TodoItem { Id = 3, Title = "Add your own business logic to controllers", IsCompleted = false }
        ];

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "Online",
                message = "ASP.NET Core Backend is running smoothly!",
                serverTimeUtc = DateTime.UtcNow,
                framework = ".NET 10.0"
            });
        }

        [HttpGet("items")]
        public ActionResult<IEnumerable<TodoItem>> GetItems()
        {
            return Ok(SampleItems);
        }

        [HttpPost("items")]
        public ActionResult<TodoItem> AddItem([FromBody] CreateItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { error = "Title cannot be empty." });
            }

            var newItem = new TodoItem
            {
                Id = SampleItems.Count > 0 ? SampleItems.Max(i => i.Id) + 1 : 1,
                Title = request.Title,
                IsCompleted = false
            };

            SampleItems.Add(newItem);
            return CreatedAtAction(nameof(GetItems), new { id = newItem.Id }, newItem);
        }

        [HttpDelete("items/{id:int}")]
        public IActionResult DeleteItem(int id)
        {
            var item = SampleItems.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound(new { error = $"Item with ID {id} not found." });
            }

            SampleItems.Remove(item);
            return Ok(new { success = true, deletedId = id });
        }
    }

    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    public class CreateItemRequest
    {
        public string Title { get; set; } = string.Empty;
    }
}
