using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

public class MedicaoInput
{
    public string Equipamento { get; set; } = string.Empty;
    public string Lote { get; set; } = string.Empty;
    public double Valor { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class CepController : ControllerBase
{
    private static readonly List<MedicaoInput> Medicoes = new();

    [HttpGet("dashboard")]
    public IActionResult GetDashboardData()
    {
        var total = Medicoes.Count;
        var media = total > 0 ? Medicoes.Average(m => m.Valor) : 25.0012;

        var data = new
        {
            Equipamento = Medicoes.LastOrDefault()?.Equipamento ?? "CNC-01",
            Lote = Medicoes.LastOrDefault()?.Lote ?? "LOT-202610-A",
            MediaGeral = Math.Round(media, 4),
            Cp = 1.33,
            Cpk = 1.25,
            Status = "Sob Controlo Estatístico",
            TotalAmostras = total > 0 ? total : 10,
            UltimasMedicoes = Medicoes.TakeLast(5).ToList()
        };

        return Ok(data);
    }

    [HttpPost("medicao")]
    public IActionResult AdicionarMedicao([FromBody] MedicaoInput medicao)
    {
        if (medicao.Valor <= 0)
            return BadRequest(new { Message = "O valor da medição deve ser maior que zero." });

        Medicoes.Add(medicao);
        return Ok(new { Message = "Medição registada com sucesso!", Total = Medicoes.Count });
    }
}
