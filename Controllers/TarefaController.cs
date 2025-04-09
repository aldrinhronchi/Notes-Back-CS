using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Services.Tarefas.Interface;

namespace Notes_Back_CS.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService TarefaService;

        public TarefaController(ITarefaService TarefaService)
        {
            this.TarefaService = TarefaService;
        }

        #region Tarefa

        [HttpGet]
        public IActionResult ListarTarefas(Int32 Pagina = 1, Int32 RegistroPorPagina = 10,
            String Campos = "", String Valores = "", String Ordenacao = "", Boolean Ordem = false)
        {
            string? Token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return Ok(this.TarefaService.Listar(Token, Pagina, RegistroPorPagina, Campos, Valores, Ordenacao, Ordem));
        }

        [HttpPost]
        public IActionResult SalvarTarefa(Tarefa userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return this.TarefaService.Salvar(userViewModel) ? Ok() : BadRequest();
        }

        [HttpDelete("{_userID}")]
        public IActionResult ExcluirTarefa(String _userID)
        {
            return Ok(this.TarefaService.Excluir(_userID));
        }

        #endregion Tarefa
    }
}