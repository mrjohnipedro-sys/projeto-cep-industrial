using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

public class InspecaoInput
{
    public string DataHora { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    public string Operador { get; set; } = string.Empty;
    public string NumeroOp { get; set; } = string.Empty;
    public string RefProduto { get; set; } = string.Empty;
    public string OrigemAco { get; set; } = string.Empty;
    public string LoteAco { get; set; } = string.Empty;
    
    // Cotas Dimensionais
    public double FuroCentro { get; set; }
    public double CotaZ { get; set; }
    public double CotaH { get; set; }
    public double Excentricidade { get; set; }
    public double Blank { get; set; }
    
    // Atributos
    public string Rebarba { get; set; } = "OK";
    public string FuroPcd { get; set; } = "OK";

    // Campos de Desvio (Preenchidos apenas se houver aprovação)
    public bool PossuiDesvio { get; set; } = false;
    public string? AprovadoPor { get; set; }
    public string? JustificativaDesvio { get; set; }
}

public class AprovaçãoRequest
{
    public string ResponsavelEmail { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class CepController : ControllerBase
{
    private static readonly List<InspecaoInput> Inspecoes = new();

    // Simulação da Base de Responsáveis Aprovadores
    private static readonly List<string> AprovadoresAutorizados = new()
    {
        "johni@empresa.com",
        "qualidade@empresa.com",
        "processo@empresa.com"
    };

    [HttpGet("dashboard")]
    public IActionResult GetDashboardData()
    {
        return Ok(Inspecoes.OrderByDescending(i => i.DataHora).ToList());
    }

    [HttpGet("desvios")]
    public IActionResult GetDesviosAprovados()
    {
        var desvios = Inspecoes
            .Where(i => i.PossuiDesvio)
            .OrderByDescending(i => i.DataHora)
            .ToList();
        return Ok(desvios);
    }

    [HttpPost("validar-aprovador")]
    public IActionResult ValidarAprovador([FromBody] AprovaçãoRequest request)
    {
        if (AprovadoresAutorizados.Contains(request.ResponsavelEmail.ToLower()) && request.Senha == "123456")
        {
            return Ok(new { Valido = true, Mensagem = "Aprovação autorizada." });
        }
        return Unauthorized(new { Valido = false, Mensagem = "Credenciais de responsável inválidas." });
    }

    [HttpPost("medicao")]
    public IActionResult AdicionarInspecao([FromBody] InspecaoInput inspecao)
    {
        if (string.IsNullOrWhiteSpace(inspecao.Operador) || string.IsNullOrWhiteSpace(inspecao.NumeroOp))
        {
            return BadRequest(new { Message = "Preencha os campos obrigatórios." });
        }

        inspecao.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Inspecoes.Add(inspecao);
        return Ok(new { Message = "Registo de inspeção salvo com sucesso!" });
    }
}
