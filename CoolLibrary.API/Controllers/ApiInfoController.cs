using Microsoft.AspNetCore.Mvc;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Controller for API information and GraphQL discovery
/// </summary>
[ApiController]
[Route("api")]
[ApiExplorerSettings(GroupName = "v1")]
public class ApiInfoController : ControllerBase
{
    /// <summary>
    /// Get API information including GraphQL endpoint
    /// </summary>
    /// <returns>API information</returns>
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetApiInfo()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        
        var info = new
        {
            ApiName = "CoolLibrary API",
            Version = "v1.0",
            Endpoints = new
            {
                REST = new
                {
                    BaseUrl = $"{baseUrl}/api",
                    Documentation = $"{baseUrl}/swagger",
                    Swagger = $"{baseUrl}/swagger",
                    Description = "RESTful API with traditional endpoints"
                },
                GraphQL = new
                {
                    Endpoint = $"{baseUrl}/graphql",
                    Playground = $"{baseUrl}/graphql",
                    Description = "GraphQL API with interactive schema explorer (Banana Cake Pop)",
                    Features = new[]
                    {
                        "Flexible queries - request only the data you need",
                        "Single endpoint for all operations",
                        "Interactive documentation and schema explorer",
                        "Real-time query validation and autocomplete"
                    },
                    ExampleQuery = @"
{
  authors {
    firstName
    lastName
    bookAuthors {
      book {
        title
        bookGenres {
          genre { name }
        }
      }
    }
  }
}"
                }
            },
            Authentication = new
            {
                Type = "JWT Bearer Token",
                Description = "Use the /api/v1/auth/login endpoint to obtain a token"
            }
        };

        return Ok(info);
    }

    /// <summary>
    /// Redirect to GraphQL Playground
    /// </summary>
    /// <returns>Redirect to GraphQL endpoint</returns>
    [HttpGet("graphql")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public IActionResult RedirectToGraphQL()
    {
        return Redirect("/graphql");
    }
}
