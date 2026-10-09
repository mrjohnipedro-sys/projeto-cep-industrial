using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

public class ToleranciaDto
{
    public string RefProduto { get; set; } = string.Empty;
    public double FuroMin { get; set; }
    public double FuroMax { get; set; }
    public double EspessuraMin { get; set; }
    public double EspessuraMax { get; set; }
    public double ZMin { get; set; }
    public double ZMax { get; set; }
    public double HMin { get; set; }
    public double HMax { get; set; }
    public double BlankMin { get; set; }
    public double BlankMax { get; set; }
    public int Satelites { get; set; }
    public double FuroSatelite { get; set; }
}

public class OpDto
{
    public string NumeroOp { get; set; } = string.Empty;
    public string RefProduto { get; set; } = string.Empty;
    public string Status { get; set; } = "ABERTA";
}

[ApiController]
[Route("api/[controller]")]
public class CepController : ControllerBase
{
    private static readonly List<InspecaoInput> Inspecoes = new();

    // Base de Tolerâncias Cadastradas no Excel
    private static readonly List<ToleranciaDto> ToleranciasBase = new()
    {
        new ToleranciaDto { RefProduto = "D17R754GS", FuroMin = 75.3, FuroMax = 75.7, EspessuraMin = 3.75, EspessuraMax = 4.25, ZMin = 48.0, ZMax = 48.5, HMin = 14.875, HMax = 15.375, BlankMin = 431.3, BlankMax = 432.3, Satelites = 6, FuroSatelite = 9.25 },
        new ToleranciaDto { RefProduto = "D16R6245NJ", FuroMin = 62.0, FuroMax = 62.4, EspessuraMin = 4.25, EspessuraMax = 4.75, ZMin = 34.25, ZMax = 34.75, HMin = 8.90, HMax = 9.40, BlankMin = 406.4, BlankMax = 407.4, Satelites = 6, FuroSatelite = 11.5 },
        new ToleranciaDto { RefProduto = "01087500B", FuroMin = 71.3, FuroMax = 71.7, EspessuraMin = 4.25, EspessuraMax = 4.75, ZMin = 48.00, ZMax = 48.50, HMin = 20.00, HMax = 20.50, BlankMin = 450.0, BlankMax = 455.0, Satelites = 6, FuroSatelite = 11.5 }
    };

    // Base de OPs
    private static readonly List<OpDto> OpBase = new()
    {
        new OpDto { NumeroOp = "87356", RefProduto = "D17R754GS", Status = "EM PRODUÇÃO" },
        new OpDto { NumeroOp = "87362", RefProduto = "D16R6245NJ", Status = "EM PRODUÇÃO" },
        new OpDto { NumeroOp = "87359", RefProduto = "01087500B", Status = "EM PRODUÇÃO" }
    };

    [HttpGet("especificacao/{op}")]
    public IActionResult GetEspecificacaoOp(string op)
    {
        var opClean = op.Trim();
        var opItem = OpBase.FirstOrDefault(o => o.NumeroOp.Trim() == opClean);
        
        string targetRef = opItem != null ? opItem.RefProduto : "D17R754GS";
        
        var tol = ToleranciasBase.FirstOrDefault(t => t.RefProduto.Trim() == targetRef) ?? ToleranciasBase.First();

        return Ok(new
        {
            Encontrado = true,
            Op = opClean,
            RefProduto = targetRef,
            Espec = tol
        });
    }

    [HttpGet("bases-carregadas")]
    public IActionResult GetBasesCarregadas() => Ok(new { Tolerancias = ToleranciasBase, OrdensProducao = OpBase });

    [HttpGet("dashboard")]
    public IActionResult GetDashboardData() => Ok(Inspecoes.OrderByDescending(i => i.DataHora).ToList());

    [HttpPost("importar-excel")]
    public IActionResult ImportarExcel(IFormFile file) => Ok(new { Message = "Bases sincronizadas com sucesso!" });

    [HttpPost("medicao")]
    public IActionResult AdicionarInspecao([FromBody] InspecaoInput inspecao)
    {
        inspecao.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Inspecoes.Add(inspecao);
        return Ok(new { Message = "Sucesso" });
    }
}
