using Notes_Back_CS.Connections.Database;
using Notes_Back_CS.Connections.Database.Repositories;
using Notes_Back_CS.Connections.Database.Repositories.Interfaces;
using Notes_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Notes_Back_CS.Services.Tarefas.Interface
{
    public interface ITarefaService
    {
        public RequisicaoViewModel<Tarefa> Listar(String Token, Int32 Pagina, Int32 RegistrosPorPagina,
            String CamposQuery = "", String ValoresQuery = "", String Ordenacao = "", Boolean Ordem = false);

        public Boolean Salvar(string token, Tarefa TarefaViewModel);

        public Boolean Excluir(String ID);
    }
}