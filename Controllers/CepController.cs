using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CepController : ControllerBase
{
    [HttpGet("dashboard")]
    public IActionResult GetDashboardData()
    {
        // Exemplo de payload com os dados de CEP estruturados
        var data = new
        {
            Equipamento = "CNC-01",
            Lote = "LOT-202610-A",
            MediaGeral = 25.0012,
            Cp = 1.33,
            Cpk = 1.25,
            Status = "Sob Controlo Estatístico",
            TotalAmostras = 10
        };

        return Ok(data);
    }
}
