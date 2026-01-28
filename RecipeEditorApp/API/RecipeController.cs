using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecipeEditorApp.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetRecipes()
        {
            // Placeholder for actual recipe retrieval logic
            var recipes = new[]
            {
                new { Id = 1, Name = "Spaghetti Bolognese" },
                new { Id = 2, Name = "Chicken Curry" }
            };
            return Ok(recipes);
        }
    }
}
