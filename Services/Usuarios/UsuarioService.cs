using Kraft_Back_CS.Extensions.Helpers;
using Microsoft.EntityFrameworkCore;
using Notes_Back_CS.Connections.Database;
using Notes_Back_CS.Connections.Database.Repositories;
using Notes_Back_CS.Connections.Database.Repositories.Interfaces;
using Notes_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Models.Usuario;
using Notes_Back_CS.Models.ViewModels;
using Notes_Back_CS.Services.Usuarios.Interface;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Notes_Back_CS.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _chaveSecreta;
        private DatabaseContext _database;

        public UsuarioService(IConfiguration configuration, DatabaseContext database)
        {
            _chaveSecreta = configuration["SecuritySettings:SecretKey"];
            _database = database;
        }

        public RequisicaoViewModel<Usuario> Listar(Int32 Pagina, Int32 RegistrosPorPagina,
             String CamposQuery = "", String ValoresQuery = "", String Ordenacao = "", Boolean Ordem = false)
        {
            RequisicaoViewModel<Usuario> Requisicao;
            using (DatabaseContext db = _database)
            {
                IQueryable<Usuario> _Usuarios = db.Usuarios;
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
                        IQueryable<Usuario> UsuarioFiltrado = _Usuarios;
                        foreach (KeyValuePair<String, String> Filtro in Filtros)
                        {
                            switch (Filtro.Key)
                            {
                                //PODEMOS INSERIR AQUI OS FILTROS DE RELACIONAMENTO EX: PELO NOME DO CARGO => WHERE(X => X.CARGO.NOME)
                                default:
                                    UsuarioFiltrado = TipografiaHelper.Filtrar(UsuarioFiltrado, Filtro.Key, Filtro.Value);
                                    break;
                            }
                        }
                        _Usuarios = UsuarioFiltrado;
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
                            _Usuarios = TipografiaHelper.Ordenar(_Usuarios, Ordenacao, Ordem);
                            break;
                    }
                }
                else
                {
                    _Usuarios = TipografiaHelper.Ordenar(_Usuarios, "ID", Ordem);
                }
                Requisicao = TipografiaHelper.FormatarRequisicao(_Usuarios, Pagina, RegistrosPorPagina);
            }
            return Requisicao;
        }

        public Boolean Salvar(Usuario UsuarioViewModel)
        {
            Validator.ValidateObject(UsuarioViewModel, new ValidationContext(UsuarioViewModel), true);
            using (DatabaseContext db = _database)
            {
                IRepository<Usuario> UsuarioRepo = new Repository<Usuario>(db);
                Usuario? _Usuario = db.Usuarios.AsNoTracking().FirstOrDefault(x => x.ID == UsuarioViewModel.ID);
                if (_Usuario == null)
                {
                    if (UsuarioViewModel.ID != 0)
                    {
                        throw new ValidationException("ID deve ser vazio!");
                    }
                    UsuarioViewModel.Senha = EncriptarSenha(UsuarioViewModel.Senha);
                    _Usuario = db.Usuarios.AsNoTracking().FirstOrDefault(x => x.Login == UsuarioViewModel.Login || x.Email == UsuarioViewModel.Email);
                    if (_Usuario == null)
                    {
                        UsuarioViewModel.DataCriado = DateTime.UtcNow;
                        UsuarioRepo.Create(UsuarioViewModel);
                    }
                    else
                    {
                        throw new ValidationException("Email/Login já cadastrado");
                    }
                }
                else
                {
                    UsuarioViewModel.DataAlterado = DateTime.UtcNow;
                    if (UsuarioViewModel.Senha == String.Empty)
                    {
                        UsuarioViewModel.Senha = _Usuario.Senha;
                    }
                    else
                    {
                        UsuarioViewModel.Senha = EncriptarSenha(UsuarioViewModel.Senha);
                    }
                    UsuarioRepo.Update(UsuarioViewModel);
                }
            }

            return true;
        }

        public Boolean Excluir(String ID)
        {
            if (!Int32.TryParse(ID, out Int32 UsuarioID))
            {
                throw new ValidationException("ID invalido!");
            }
            using (DatabaseContext db = _database)
            {
                IRepository<Usuario> UsuarioRepo = new Repository<Usuario>(db);

                Usuario? _Usuario = UsuarioRepo.Find(UsuarioID);
                if (_Usuario == null)
                {
                    throw new ValidationException("Usuario não encontrado");
                }
                if (UsuarioRepo.Delete(_Usuario))
                {
                    List<Tarefa> Tarefas = db.Tarefas.Where(x => x.IDUsuario == UsuarioID).ToList();
                    IRepository<Tarefa> TarefaRepo = new Repository<Tarefa>(db);
                    foreach(Tarefa? Tarefa in Tarefas)
                    {
                        TarefaRepo.Delete(Tarefa);
                    }
                } else
                {
                    throw new Exception("Não foi Possivel excluir, tente novamente");

                }
            }
            return true;
        }

        public RequisicaoViewModel<Usuario> Autenticar(LoginViewModel Requisicao)
        {
            if (String.IsNullOrWhiteSpace(Requisicao.Login) || String.IsNullOrWhiteSpace(Requisicao.Senha))
            {
                throw new ValidationException("Login/Senha obrigatorios.");
            }

            Requisicao.Senha = EncriptarSenha(Requisicao.Senha);
            List<Usuario>? Usuarios = null;
            using (DatabaseContext db = _database)
            {
                Usuarios = db.Usuarios.Where(x => (x.Email.ToUpper() == Requisicao.Login.ToUpper() || x.Login.ToUpper() == Requisicao.Login.ToUpper())
                                                 && x.Ativo).ToList();
            }
            if (Usuarios == null || Usuarios.Count() == 0)
            {
                throw new ValidationException("Usuario não encontrado");
            }

            Usuario? usuario = Usuarios.FirstOrDefault(x => x.Senha == Requisicao.Senha);
            if (usuario == null)
            {
                throw new ValidationException("Senha Incorreta");
            }

            usuario.Token = TokenHelper.GerarToken(usuario);
            RequisicaoViewModel<Usuario> requisicao = new RequisicaoViewModel<Usuario>()
            {
                Data = new List<Usuario>() { usuario },
                Page = 1,
                PageSize = 10,
                Type = nameof(Usuario)
            };
            return requisicao;
        }

        

        private String EncriptarSenha(String Senha)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_chaveSecreta)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Senha));

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte caracter in hash)
                {
                    stringBuilder.Append(caracter.ToString("X2"));
                }

                return stringBuilder.ToString();
            }
        }
    }
}