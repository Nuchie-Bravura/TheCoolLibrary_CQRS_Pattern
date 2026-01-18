using Microsoft.AspNetCore.Mvc;

namespace CoolLibrary.API.Controllers;

/// <summary>
[ApiExplorerSettings(GroupName = "v1")]
public class ApiInfoController : ControllerBase
{
    /// <summary>
    /// Get API information
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


}
