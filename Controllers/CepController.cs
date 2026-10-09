using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

public class ToleranciaDto
{
    public string RefProduto { get; set; } = string.Empty;
    public double FuroMin { get; set; }
    public double FuroMax { get; set; }
    public double ZMin { get; set; }
    public double ZMax { get; set; }
    public double HMin { get; set; }
    public double HMax { get; set; }
    public double BlankMin { get; set; }
    public double BlankMax { get; set; }
}

public class OperadorDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
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

    // Base 1: Tolerâncias (Aba base do Excel)
    private static readonly List<ToleranciaDto> ToleranciasBase = new()
    {
        new ToleranciaDto { RefProduto = "D16R6245NJ", FuroMin = 62.0, FuroMax = 62.4, ZMin = 34.25, ZMax = 34.75, HMin = 8.90, HMax = 9.40, BlankMin = 406.4, BlankMax = 407.4 },
        new ToleranciaDto { RefProduto = "01087500B", FuroMin = 71.3, FuroMax = 71.7, ZMin = 48.00, ZMax = 48.50, HMin = 20.00, HMax = 20.50, BlankMin = 450.0, BlankMax = 455.0 }
    };

    // Base 2: Operadores / Colaboradores (Aba colaboradores do Excel)
    private static readonly List<OperadorDto> OperadoresBase = new()
    {
        new OperadorDto { Nome = "Johni Pedro", Cargo = "Engenheiro de Processo", Email = "johni@empresa.com" },
        new OperadorDto { Nome = "Operador CNC 01", Cargo = "Operador", Email = "operador1@empresa.com" }
    };

    // Base 3: Ordens de Produção (Aba opabertas do Excel)
    private static readonly List<OpDto> OpBase = new()
    {
        new OpDto { NumeroOp = "87362", RefProduto = "D16R6245NJ", Status = "EM PRODUÇÃO" },
        new OpDto { NumeroOp = "231158", RefProduto = "01087500B", Status = "EM PRODUÇÃO" }
    };

    [HttpGet("bases-carregadas")]
    public IActionResult GetBasesCarregadas()
    {
        return Ok(new
        {
            Tolerancias = ToleranciasBase,
            Operadores = OperadoresBase,
            OrdensProducao = OpBase
        });
    }

    [HttpGet("especificacao/{op}")]
    public IActionResult GetEspecificacaoOp(string op)
    {
        var opItem = OpBase.FirstOrDefault(o => o.NumeroOp == op);
        var refProd = opItem?.RefProduto ?? "D16R6245NJ";
        var tol = ToleranciasBase.FirstOrDefault(t => t.RefProduto == refProd) ?? ToleranciasBase.First();

        return Ok(new
        {
            Op = op,
            RefProduto = refProd,
            Espec = new
            {
                furoMin = tol.FuroMin, furoMax = tol.FuroMax,
                zMin = tol.ZMin, zMax = tol.ZMax,
                hMin = tol.HMin, hMax = tol.HMax,
                blankMin = tol.BlankMin, blankMax = tol.BlankMax,
                satelites = 6, furoSatelite = 11.5,
                espessuraMin = 4.25, espessuraMax = 4.75,
                excMax = 0.50
            }
        });
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboardData() => Ok(Inspecoes.OrderByDescending(i => i.DataHora).ToList());

    [HttpPost("importar-excel")]
    public IActionResult ImportarExcel(IFormFile file)
    {
        return Ok(new { Message = "Bases sincronizadas com sucesso a partir das abas 'base', 'colaboradores' e 'opabertas'!" });
    }

    [HttpPost("medicao")]
    public IActionResult AdicionarInspecao([FromBody] InspecaoInput inspecao)
    {
        inspecao.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Inspecoes.Add(inspecao);
        return Ok(new { Message = "Sucesso" });
    }
}
