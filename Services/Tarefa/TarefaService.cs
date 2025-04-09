using Microsoft.EntityFrameworkCore;
using Notes_Back_CS.Connections.Database;
using Notes_Back_CS.Connections.Database.Repositories;
using Notes_Back_CS.Connections.Database.Repositories.Interfaces;
using Notes_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Models.ViewModels;
using Notes_Back_CS.Services.Tarefas.Interface;
using Notes_Back_CS.Services.Usuarios;
using System.ComponentModel.DataAnnotations;

namespace Notes_Back_CS.Services.Tarefas
{
    public class TarefaService : ITarefaService
    {
        private DatabaseContext _database;

        public TarefaService(DatabaseContext database)
        {
            _database = database;
        }

        public RequisicaoViewModel<Tarefa> Listar(String Token, Int32 Pagina, Int32 RegistrosPorPagina,
             String CamposQuery = "", String ValoresQuery = "", String Ordenacao = "", Boolean Ordem = false)
        {
            String? usuarioId = UsuarioService.ObterIDUsuarioDeToken(Token);
            if (usuarioId == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado no token!");
            }

            RequisicaoViewModel<Tarefa> Requisicao;
            using (DatabaseContext db = _database)
            {
                IQueryable<Tarefa> _Tarefas = db.Tarefas;
                if (!String.IsNullOrWhiteSpace(CamposQuery))
                {
                    String[] CamposArray = CamposQuery.Split(";|;");
                    String[] ValoresArray = ValoresQuery.Split(";|;");
                    if (CamposArray.Length == ValoresArray.Length)
                    {
                        Dictionary<String, String> Filtros = new Dictionary<String, String>();
                        for (Int32 index = 0; index < CamposArray.Length; index++)
                        {
                            String? Campo = CamposArray[index];
                            String? Valor = ValoresArray[index];
                            if (!(String.IsNullOrWhiteSpace(Campo) && String.IsNullOrWhiteSpace(Valor)))
                            {
                                Filtros.Add(Campo, Valor);
                            }
                        }
                        IQueryable<Tarefa> TarefaFiltrado = _Tarefas;
                        foreach (KeyValuePair<String, String> Filtro in Filtros)
                        {
                            switch (Filtro.Key)
                            {
                                //PODEMOS INSERIR AQUI OS FILTROS DE RELACIONAMENTO EX: PELO NOME DO CARGO => WHERE(X => X.CARGO.NOME)
                                default:
                                    TarefaFiltrado = TipografiaHelper.Filtrar(TarefaFiltrado, Filtro.Key, Filtro.Value);
                                    break;
                            }
                        }
                        _Tarefas = TarefaFiltrado;
                    }
                    else
                    {
                        throw new ValidationException("Não foi possivel filtrar!");
                    }
                }
                if (!String.IsNullOrWhiteSpace(Ordenacao))
                {
                    switch (Ordenacao)
                    {
                        default:
                            _Tarefas = TipografiaHelper.Ordenar(_Tarefas, Ordenacao, Ordem);
                            break;
                    }
                }
                else
                {
                    _Tarefas = TipografiaHelper.Ordenar(_Tarefas, "Fixado", Ordem);
                    _Tarefas = TipografiaHelper.Ordenar(_Tarefas, "ID", Ordem);
                }
                Requisicao = TipografiaHelper.FormatarRequisicao(_Tarefas, Pagina, RegistrosPorPagina);
            }
            return Requisicao;
        }

        public Boolean Salvar(Tarefa TarefaViewModel)
        {
            Validator.ValidateObject(TarefaViewModel, new ValidationContext(TarefaViewModel), true);
            using (DatabaseContext db = _database)
            {
                IRepository<Tarefa> CargoRepo = new Repository<Tarefa>(db);
                Tarefa? _Cargo = db.Tarefas.AsNoTracking().FirstOrDefault(x => x.ID == TarefaViewModel.ID);
                if (_Cargo == null)
                {
                    if (TarefaViewModel.ID != 0)
                    {
                        throw new ValidationException("ID deve ser vazio!");
                    }
                    TarefaViewModel.DataCriado = DateTime.UtcNow;
                    CargoRepo.Create(TarefaViewModel);
                }
                else
                {
                    TarefaViewModel.DataAlterado = DateTime.UtcNow;
                    CargoRepo.Update(TarefaViewModel);
                }
            }

            return true;
        }

        public Boolean Excluir(String ID)
        {
            if (!Int32.TryParse(ID, out Int32 TarefaID))
            {
                throw new ValidationException("ID invalido!");
            }
            using (DatabaseContext db = _database)
            {
                IRepository<Tarefa> TarefaRepo = new Repository<Tarefa>(db);

                Tarefa? _Tarefa = TarefaRepo.Find(TarefaID);
                if (_Tarefa == null)
                {
                    throw new ValidationException("Tarefa não encontrado");
                }
                return TarefaRepo.Delete(_Tarefa);
            }
        }
    }
}