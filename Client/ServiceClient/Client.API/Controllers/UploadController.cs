using Client.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class UploadController(ImportClientsCsvUseCase useCase) : ControllerBase
{
    [HttpPost("import-csv")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportCsv(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Aucun fichier fourni ou fichier vide." });

        if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Le fichier doit être de type CSV." });

        var result = await useCase.ExecuteAsync(file.OpenReadStream(), cancellationToken);

        return Ok(result);
    }
}

