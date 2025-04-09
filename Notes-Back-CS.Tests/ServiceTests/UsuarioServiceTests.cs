using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Notes_Back_CS.Extensions.Middlewares;
using Notes_Back_CS.Models.Usuario;
using Notes_Back_CS.Models.ViewModels;
using Notes_Back_CS.Services.Usuarios;
using Notes_Back_CS.Tests.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes_Back_CS.Tests.ServiceTests
{
    public class UsuarioServiceTests
    {
        [Fact]
        public void Listar_DeveRetornarUsuariosCorretamente()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            dbContext.Usuarios.Add(new Usuario { ID = 42, Nome = "Usuário Teste", Login = "Teste", Email = "teste@email.com", Senha = TestHelper.EncriptarSenha("Senha123"), Ativo = true });
            dbContext.SaveChanges();

            var configMock = TestHelper.CriarConfigMock();
            dbContext = TestHelper.CriarDbContextFake();
            var service = new UsuarioService(configMock.Object, dbContext); var resultado = service.Listar(1, 10);

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado.Data);
            Assert.Equal("Usuário Teste", resultado.Data[0].Nome);
        }

        [Fact]
        public void Salvar_DeveAdicionarNovoUsuario()
        {
            var configMock = TestHelper.CriarConfigMock();
            var dbContext = TestHelper.CriarDbContextFake();
            var service = new UsuarioService(configMock.Object, dbContext);

            var novoUsuario = new Usuario { Nome = "Novo Usuário", Email = "novo1@email.com", Login = "Teste", Senha = TestHelper.EncriptarSenha("Senha123"), Ativo = true };
            var resultado = service.Salvar(novoUsuario);

            var validacaoDbContext = TestHelper.CriarDbContextFake();
            var usuarioSalvo = validacaoDbContext.Usuarios.FirstOrDefault(u => u.Email == "novo1@email.com");

            Assert.True(resultado);
            Assert.NotNull(usuarioSalvo);
        }

        [Fact]
        public void Excluir_DeveRemoverUsuarioExistente()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            dbContext.Usuarios.Add(new Usuario { ID = 126, Nome = "Usuário Teste", Login = "Teste", Email = "teste@email.com", Senha = TestHelper.EncriptarSenha("Senha123"), Ativo = true });
            dbContext.SaveChanges();

            var configMock = TestHelper.CriarConfigMock();
            dbContext = TestHelper.CriarDbContextFake();
            var service = new UsuarioService(configMock.Object, dbContext);

            var resultado = service.Excluir("1");

            var validacaoDbContext = TestHelper.CriarDbContextFake();
            var usuarioRemovido = validacaoDbContext.Usuarios.FirstOrDefault(u => u.ID == 1);

            Assert.True(resultado);
            Assert.Null(usuarioRemovido);
        }

        [Fact]
        public void Autenticar_DeveRetornarTokenParaUsuarioValido()
        {
            var configMock = TestHelper.CriarConfigMock();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(configMock.Object);

            // Criar ServiceProvider e registrar no ServiceLocator
            var serviceProvider = serviceCollection.BuildServiceProvider();
            ServiceLocator.IncluirServico(serviceProvider);

            var dbContext = TestHelper.CriarDbContextFake();
            var service = new UsuarioService(configMock.Object, dbContext);

            var usuario = new Usuario
            {
                ID = 1,
                Nome = "Usuário Teste",
                Email = "teste@email.com",
                Login = "teste",
                Senha = TestHelper.EncriptarSenha("Senha123"),
                Ativo = true
            };

            dbContext.Usuarios.Add(usuario);
            dbContext.SaveChanges();

            var loginViewModel = new LoginViewModel
            {
                Login = "teste",
                Senha = "Senha123"
            };

            var resultado = service.Autenticar(loginViewModel);

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado.Data);
            Assert.NotNull(resultado.Data[0].Token);
        }

        [Fact]
        public void ObterIDUsuarioDeToken_DeveRetornarIdCorreto()
        {
            var token = TestHelper.GerarJwtFake();
            var resultado = UsuarioService.ObterIDUsuarioDeToken(token);

            Assert.Equal("123", resultado);
        }

        [Fact]
        public void Salvar_DeveLancarExcecao_QuandoEmailJaCadastrado()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            dbContext.Usuarios.Add(new Usuario { Nome = "Usuário Teste", Email = "teste@email.com", Login = "Teste123", Senha = "Senha123", Ativo = true });
            dbContext.SaveChanges();

            var configMock = TestHelper.CriarConfigMock();
            dbContext = TestHelper.CriarDbContextFake();
            var service = new UsuarioService(configMock.Object, dbContext); var usuarioDuplicado = new Usuario { Nome = "Outro Usuário", Email = "teste@email.com", Senha = "Senha123", Ativo = true };

            Assert.Throws<ValidationException>(() => service.Salvar(usuarioDuplicado));
        }
    }
}