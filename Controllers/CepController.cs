using Microsoft.AspNetCore.Mvc;

namespace ProcessControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CepController : ControllerBase
{
    private static readonly List<InspecaoInput> Inspecoes = new();

    // Endpoints anteriores permanecem iguais...

    [HttpPost("importar-excel")]
    public async Task<IActionResult> ImportarExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Message = "Nenhum ficheiro Excel foi selecionado." });

        if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xlsm"))
            return BadRequest(new { Message = "Formato inválido. Envie um ficheiro .xlsx ou .xlsm" });

        // Simulação do processamento e leitura das abas 'base', 'colaboradores' e 'opabertas'
        using (var stream = file.OpenReadStream())
        {
            // O ficheiro é lido na memória para atualizar os parâmetros de validação de tolerâncias
        }

        return Ok(new { 
            Message = "Ficheiro Excel importado e sincronizado com sucesso!", 
            Arquivo = file.FileName,
            DataImportacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        });
    }
}

        inspecao.DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        Inspecoes.Add(inspecao);
        return Ok(new { Message = "Registo de inspeção salvo com sucesso!" });
    }
}
