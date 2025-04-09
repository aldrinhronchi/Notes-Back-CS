using Kraft_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Connections.Database;
using Notes_Back_CS.Connections.Database.Repositories;
using Notes_Back_CS.Connections.Database.Repositories.Interfaces;
using Notes_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Models.Usuario;
using Notes_Back_CS.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Notes_Back_CS.Services.Usuarios.Interface
{
    public interface IUsuarioService
    {
        public RequisicaoViewModel<Usuario> Listar(Int32 Pagina, Int32 RegistrosPorPagina,
             String CamposQuery = "", String ValoresQuery = "", String Ordenacao = "", Boolean Ordem = false);

        public Boolean Salvar(Usuario UsuarioViewModel);

        public Boolean Excluir(String ID);

        public RequisicaoViewModel<Usuario> Autenticar(LoginViewModel Requisicao);
    }
}