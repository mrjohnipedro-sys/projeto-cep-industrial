using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

public class InspecaoInput
{
    public string DataHora { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    public string Operador { get; set; } = string.Empty;
    public string NumeroOpi { get; set; } = string.Empty;
    public string RefProduto { get; set; } = string.Empty;
    public string OrigemAco { get; set; } = string.Empty;
    public string LoteAco { get; set; } = string.Empty;
    
    // Cotas e Parâmetros Dimensional
    public double FuroCentro { get; set; }
    public double CotaZ { get; set; }
    public string Rebarba { get; set; } = "OK";
    public double FuroPcd { get; set; }
    public double Espessura { get; set; }
    public double CotaH { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class CepController : ControllerBase
{
    private static readonly List<InspecaoInput> Inspecoes = new();

    [HttpGet("dashboard")]
    public IActionResult GetDashboardData()
    {
        return Ok(Inspecoes.OrderByDescending(i => i.DataHora).ToList());
    }

    [HttpPost("medicao")]
    public IActionResult AdicionarInspecao([FromBody] InspecaoInput inspecao)
    {
        if (string.IsNullOrWhiteSpace(inspecao.Operador) || string.IsNullOrWhiteSpace(inspecao.NumeroOpi))
        {
            return BadRequest(new { Message = "Por favor, preencha todos os campos antes de enviar o registo." });
        }

        inspecao.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Inspecoes.Add(inspecao);
        return Ok(new { Message = "Inspeção de Processo salva com sucesso!" });
    }
}
