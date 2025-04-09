using Microsoft.AspNetCore.Mvc;
using Moq;
using Notes_Back_CS.Controllers;
using Notes_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Models.Usuario;
using Notes_Back_CS.Models.ViewModels;
using Notes_Back_CS.Services.Usuarios.Interface;
using Notes_Back_CS.Tests.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes_Back_CS.Tests.ControllerTests
{
    public class UsuarioControllerTests
    {
        [Fact]
        public void ListarUsuarios_DeveRetornarOkComRequisicaoViewModel()
        {
            var mockUsuarioService = new Mock<IUsuarioService>();
            int Pagina = 1;
            int RegistrosPorPagina = 10;

            var usuariosFake = new List<Usuario>
            {
                new Usuario { ID = 1, Nome = "Usuário 1", Email = "usuario1@email.com" },
                new Usuario { ID = 2, Nome = "Usuário 2", Email = "usuario2@email.com" }
            };

            var requisicaoMock = new RequisicaoViewModel<Usuario>
            {
                Data = usuariosFake,
                Type = nameof(Usuario),
                Page = Pagina,
                PageSize = RegistrosPorPagina,
                PageCount = 1,
            };

            mockUsuarioService.Setup(s => s.Listar(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                              .Returns(requisicaoMock);

            var controller = new UsuarioController(mockUsuarioService.Object);
            var result = controller.ListarUsuarios();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var requisicaoRetornada = Assert.IsType<RequisicaoViewModel<Usuario>>(okResult.Value);

            Assert.Equal(2, requisicaoRetornada.Data.Count);
        }

        [Fact]
        public void SalvarUsuario_ComDadosValidos_DeveRetornarMensagemDeSucesso()
        {
            var mockUsuarioService = new Mock<IUsuarioService>();

            mockUsuarioService.Setup(s => s.Salvar(It.IsAny<Usuario>())).Returns(true);

            var controller = new UsuarioController(mockUsuarioService.Object);
            var usuarioFake = new Usuario { Nome = "Novo Usuário", Email = "novo@email.com" };

            var result = controller.SalvarUsuario(usuarioFake);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ExcluirUsuario_DeveRetornarMensagemDeSucesso()
        {
            var mockUsuarioService = new Mock<IUsuarioService>();

            var respostaMock = new RequisicaoViewModel<Usuario> { Status = "Sucesso", Type = nameof(Usuario) };

            mockUsuarioService.Setup(s => s.Excluir(It.IsAny<string>())).Returns(true);

            var controller = new UsuarioController(mockUsuarioService.Object);
            var result = controller.ExcluirUsuario("123");

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Autenticar_DeveRetornarOkComUsuarioEToken()
        {
            var mockUsuarioService = new Mock<IUsuarioService>();
            var controller = new UsuarioController(mockUsuarioService.Object);

            var loginFake = new LoginViewModel { Login = "teste@email.com", Senha = "senha123" };
            var usuarioMock = new Usuario { ID = 1, Nome = "Usuário Teste", Email = "teste@email.com", Token = "jwt-token-fake" };

            var respostaMock = new RequisicaoViewModel<Usuario>
            {
                Data = new List<Usuario> { usuarioMock },
                Type = nameof(Usuario),
            };

            mockUsuarioService.Setup(s => s.Autenticar(It.IsAny<LoginViewModel>())).Returns(respostaMock);

            var result = controller.Autenticar(loginFake);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var requisicaoRetornada = Assert.IsType<RequisicaoViewModel<Usuario>>(okResult.Value);

            Assert.Single(requisicaoRetornada.Data);
            Assert.Equal("jwt-token-fake", requisicaoRetornada.Data[0].Token);
        }
    }
}