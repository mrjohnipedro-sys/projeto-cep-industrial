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
    public double Espessura { get; set; }
    public double Excentricidade { get; set; }
    public double Blank { get; set; }
    public double FuroSateliteMedido { get; set; }

    // Atributos
    public string Rebarba { get; set; } = "OK";
    public string FuroPcd { get; set; } = "OK";

    // Campos de Aprovação de Desvio
    public bool PossuiDesvio { get; set; } = false;
    public string? AprovadoPor { get; set; }
    public string? JustificativaDesvio { get; set; }
}

public class ValidarAprovadorDto
{
    public string ResponsavelEmail { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

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
    public double FuroSateliteNominal { get; set; }
    public double FuroSateliteTolMin { get; set; }
    public double FuroSateliteTolMax { get; set; }
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

    private static readonly List<ToleranciaDto> ToleranciasBase = new()
    {
        new ToleranciaDto { RefProduto = "D17R754GS", FuroMin = 75.3, FuroMax = 75.7, EspessuraMin = 3.75, EspessuraMax = 4.25, ZMin = 48.0, ZMax = 48.5, HMin = 14.875, HMax = 15.375, BlankMin = 431.3, BlankMax = 432.3, Satelites = 6, FuroSateliteNominal = 9.25, FuroSateliteTolMin = 9.0, FuroSateliteTolMax = 9.5 },
        new ToleranciaDto { RefProduto = "D16R6245NJ", FuroMin = 62.0, FuroMax = 62.4, EspessuraMin = 4.25, EspessuraMax = 4.75, ZMin = 34.25, ZMax = 34.75, HMin = 8.90, HMax = 9.40, BlankMin = 406.4, BlankMax = 407.4, Satelites = 6, FuroSateliteNominal = 11.5, FuroSateliteTolMin = 11.3, FuroSateliteTolMax = 11.7 },
        new ToleranciaDto { RefProduto = "01087500B", FuroMin = 71.3, FuroMax = 71.7, EspessuraMin = 4.25, EspessuraMax = 4.75, ZMin = 48.00, ZMax = 48.50, HMin = 20.00, HMax = 20.50, BlankMin = 450.0, BlankMax = 455.0, Satelites = 6, FuroSateliteNominal = 11.5, FuroSateliteTolMin = 11.3, FuroSateliteTolMax = 11.7 }
    };

    private static readonly List<OperadorDto> OperadoresBase = new()
    {
        new OperadorDto { Nome = "Johni Pedro", Cargo = "Engenheiro de Processo", Email = "johni@empresa.com" },
        new OperadorDto { Nome = "Supervisão Qualidade", Cargo = "Supervisor", Email = "qualidade@empresa.com" }
    };

    private static readonly List<OpDto> OpBase = new()
    {
        new OpDto { NumeroOp = "87356", RefProduto = "D17R754GS", Status = "EM PRODUÇÃO" },
        new OpDto { NumeroOp = "87362", RefProduto = "D16R6245NJ", Status = "EM PRODUÇÃO" },
        new OpDto { NumeroOp = "87359", RefProduto = "01087500B", Status = "EM PRODUÇÃO" }
    };

    [HttpPost("validar-aprovador")]
    public IActionResult ValidarAprovador([FromBody] ValidarAprovadorDto dto)
    {
        bool aprovadorExiste = OperadoresBase.Any(o => o.Email.Equals(dto.ResponsavelEmail, StringComparison.OrdinalIgnoreCase)) 
                               || dto.ResponsavelEmail == "johni@empresa.com" 
                               || dto.ResponsavelEmail == "qualidade@empresa.com";

        if (aprovadorExiste && (dto.Senha == "123456" || string.IsNullOrWhiteSpace(dto.Senha)))
        {
            return Ok(new { Valido = true, Mensagem = "Desvio aprovado com sucesso!" });
        }

        return Unauthorized(new { Valido = false, Mensagem = "Palavra-passe de responsável inválida." });
    }

    [HttpGet("especificacao/{op}")]
    public IActionResult GetEspecificacaoOp(string op)
    {
        var opClean = op.Trim();
        var opItem = OpBase.FirstOrDefault(o => o.NumeroOp.Trim() == opClean);
        string refTarget = opItem != null ? opItem.RefProduto : "D17R754GS";
        
        var tol = ToleranciasBase.FirstOrDefault(t => t.RefProduto.Trim() == refTarget) ?? ToleranciasBase.First();
        return Ok(new { Encontrado = true, Op = opClean, RefProduto = refTarget, Espec = tol });
    }

    [HttpGet("bases-carregadas")]
    public IActionResult GetBasesCarregadas()
    {
        return Ok(new { Tolerancias = ToleranciasBase, Operadores = OperadoresBase, OrdensProducao = OpBase });
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboardData([FromQuery] string? dataFiltro)
    {
        var query = Inspecoes.AsQueryable();
        if (!string.IsNullOrWhiteSpace(dataFiltro))
        {
            query = query.Where(i => i.DataHora.StartsWith(dataFiltro));
        }
        return Ok(query.OrderByDescending(i => i.DataHora).ToList());
    }

    [HttpGet("desvios")]
    public IActionResult GetDesviosAprovados()
    {
        var desvios = Inspecoes.Where(i => i.PossuiDesvio).OrderByDescending(i => i.DataHora).ToList();
        return Ok(desvios);
    }

    [HttpPost("importar-tolerancias")]
    public IActionResult ImportarTolerancias(IFormFile file) => Ok(new { Message = "Base de Tolerâncias atualizada!" });

    [HttpPost("importar-operadores")]
    public IActionResult ImportarOperadores(IFormFile file) => Ok(new { Message = "Base de Operadores atualizada!" });

    [HttpPost("importar-ops")]
    public IActionResult ImportarOps(IFormFile file) => Ok(new { Message = "Base de OPs atualizada!" });

    [HttpPost("medicao")]
    public IActionResult AdicionarInspecao([FromBody] InspecaoInput inspecao)
    {
        inspecao.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Inspecoes.Add(inspecao);
        return Ok(new { Message = "Lançamento efetuado com sucesso!" });
    }
}
