using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Notes_Back_CS.Connections.Database;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Services.Tarefas;
using Notes_Back_CS.Tests.Helper;
using System.ComponentModel.DataAnnotations;

namespace Notes_Back_CS.Tests.ServiceTests
{
    public class TarefaServiceTests
    {
        [Fact]
        public void Listar_DeveRetornarTarefasParaUsuario()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            dbContext.Tarefas.Add(new Tarefa
            {
                Titulo = "Teste Tarefa",
                IDUsuario = 123,
                Conteudo = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In a nibh efficitur, gravida nibh nec, pharetra lorem. Nulla facilisi. " +
                 "Phasellus condimentum tortor a justo dapibus ullamcorper. Cras pharetra vitae mauris vel semper. Suspendisse cursus leo urna, " +
                 "eu tincidunt lorem auctor quis. Aenean turpis est, placerat sed ligula at, maximus ultricies elit. Integer urna turpis," +
                 " consectetur at lectus ut, lacinia sollicitudin eros. Phasellus sapien libero, " +
                 "finibus eget sem sit amet, laoreet dictum enim. Sed ac blandit elit. Morbi viverra commodo vestibulum. In hac habitasse platea dictumst."
            });
            dbContext.SaveChanges();

            var service = new TarefaService(dbContext);
            var tokenValido = TestHelper.GerarJwtFake();
            var resultado = service.Listar(tokenValido, 1, 10);

            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado.Data);
            Assert.Equal("Teste Tarefa", resultado.Data[0].Titulo);
        }

        [Fact]
        public void Listar_DeveRetornarErroComTokenInvalido()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            var service = new TarefaService(dbContext);

            var tokenInvalido = "TokenInvalido123";

            var exception = Assert.Throws<SecurityTokenMalformedException>(() => service.Listar(tokenInvalido, 1, 10));

            Assert.Contains("IDX12709", exception.Message);
        }

        [Fact]
        public void Salvar_DeveAdicionarNovaTarefa()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            var service = new TarefaService(dbContext);

            var novaTarefa = new Tarefa
            {
                Titulo = "Nova Tarefa",
                IDUsuario = 123,
                Conteudo = "Texto de teste"
            };

            var resultado = service.Salvar(novaTarefa);

            var validacaoDbContext = TestHelper.CriarDbContextFake();
            var tarefaSalva = validacaoDbContext.Tarefas.FirstOrDefault(t => t.Titulo == "Nova Tarefa");

            Assert.True(resultado);
            Assert.NotNull(tarefaSalva);
        }

        [Fact]
        public void Salvar_DeveLancarExcecaoSeIDForInvalido()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            var service = new TarefaService(dbContext);

            var tarefaComIDInvalido = new Tarefa
            {
                ID = 999,
                Titulo = "Tarefa Teste",
                IDUsuario = 123,
                Conteudo = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In a nibh efficitur, gravida nibh nec, pharetra lorem. Nulla facilisi. " +
                "Phasellus condimentum tortor a justo dapibus ullamcorper. Cras pharetra vitae mauris vel semper. Suspendisse cursus leo urna, " +
                "eu tincidunt lorem auctor quis. Aenean turpis est, placerat sed ligula at, maximus ultricies elit. Integer urna turpis," +
                " consectetur at lectus ut, lacinia sollicitudin eros. Phasellus sapien libero, " +
                "finibus eget sem sit amet, laoreet dictum enim. Sed ac blandit elit. Morbi viverra commodo vestibulum. In hac habitasse platea dictumst."
            };

            Assert.Throws<ValidationException>(() => service.Salvar(tarefaComIDInvalido));
        }

        [Fact]
        public void Excluir_DeveRemoverTarefaExistente()
        {
            var dbContext = TestHelper.CriarDbContextFake();
            dbContext.Tarefas.Add(new Tarefa
            {
                ID = 2290,
                Titulo = "Teste Tarefa",
                IDUsuario = 123,
                Conteudo = "Texto de teste"
            });
            dbContext.SaveChanges();

            var novoDbContext = TestHelper.CriarDbContextFake();
            var service = new TarefaService(novoDbContext);

            var resultado = service.Excluir("2290");

            var validacaoDbContext = TestHelper.CriarDbContextFake();
            var tarefaRemovida = validacaoDbContext.Tarefas.FirstOrDefault(t => t.ID == 2290);

            Assert.True(resultado);
            Assert.Null(tarefaRemovida);
        }
    }
}