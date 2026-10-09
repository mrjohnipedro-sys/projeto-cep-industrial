using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

public class ProdutoEspecificacao
{
    public string RefProduto { get; set; } = string.Empty;
    public double FuroMin { get; set; }
    public double FuroMax { get; set; }
    public double ZMin { get; set; }
    public double ZMax { get; set; }
    public double EspessuraMin { get; set; }
    public double EspessuraMax { get; set; }
    public double HMin { get; set; }
    public double HMax { get; set; }
    public double ExcMax { get; set; }
    public double BlankMin { get; set; }
    public double BlankMax { get; set; }
    public int Satelites { get; set; }
    public double FuroSatelite { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class CepController : ControllerBase
{
    private static readonly List<InspecaoInput> Inspecoes = new();

    // Dicionário simulando a base carregada via Excel
    private static readonly Dictionary<string, (string Ref, ProdutoEspecificacao Espec)> BaseOp = new()
    {
        { "87362", ("D16R6245NJ", new ProdutoEspecificacao { 
            RefProduto = "D16R6245NJ",
            FuroMin = 62.0, FuroMax = 62.4,
            ZMin = 34.25, ZMax = 34.75,
            EspessuraMin = 4.25, EspessuraMax = 4.75,
            HMin = 8.90, HMax = 9.40,
            ExcMax = 0.50,
            BlankMin = 406.4, BlankMax = 407.4,
            Satelites = 6, FuroSatelite = 11.5
        })}
    };

    [HttpGet("especificacao/{op}")]
    public IActionResult GetEspecificacaoOp(string op)
    {
        if (BaseOp.TryGetValue(op, out var item))
        {
            return Ok(new { Op = op, RefProduto = item.Ref, Espec = item.Espec });
        }

        // Especificação Padrão caso a OP não esteja cadastrada
        return Ok(new { Op = op, RefProduto = "DESCONHECIDO", Espec = new ProdutoEspecificacao { 
            FuroMin = 71.30, FuroMax = 71.70,
            ZMin = 48.00, ZMax = 48.50,
            EspessuraMin = 4.25, EspessuraMax = 4.75,
            HMin = 20.00, HMax = 20.50,
            ExcMax = 0.50,
            BlankMin = 450.00, BlankMax = 455.00,
            Satelites = 6, FuroSatelite = 11.5
        }});
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboardData() => Ok(Inspecoes.OrderByDescending(i => i.DataHora).ToList());

    [HttpGet("desvios")]
    public IActionResult GetDesviosAprovados() => Ok(Inspecoes.Where(i => i.PossuiDesvio).OrderByDescending(i => i.DataHora).ToList());

    [HttpPost("validar-aprovador")]
    public IActionResult ValidarAprovador([FromBody] AprovaçãoRequest request)
    {
        if (request.Senha == "123456") return Ok(new { Valido = true });
        return Unauthorized(new { Valido = false });
    }

    [HttpPost("medicao")]
    public IActionResult AdicionarInspecao([FromBody] InspecaoInput inspecao)
    {
        inspecao.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Inspecoes.Add(inspecao);
        return Ok(new { Message = "Sucesso" });
    }
}
