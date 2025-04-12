using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Notes_Back_CS.Controllers;
using Notes_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Models.ViewModels;
using Notes_Back_CS.Services.Tarefas.Interface;
using Notes_Back_CS.Tests.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes_Back_CS.Tests.ControllerTests
{
    public class TarefaControllerTests
    {
        [Fact]
        public void ListarTarefas_DeveRetornarOkComListaDeTarefas()
        {
            var mockTarefaService = new Mock<ITarefaService>();

            int IDUsuario = 100;
            int Pagina = 1;
            int RegistrosPorPagina = 10;

            var tarefasFake = new List<Tarefa>
            {
                new Tarefa { ID = 1, IDUsuario = IDUsuario, Titulo = "Tarefa 1", Conteudo = "Descrição 1", Fixado = false, Concluido = true },
                new Tarefa { ID = 2, IDUsuario = IDUsuario, Titulo = "Tarefa 2", Conteudo = "Descrição 2", Fixado = true, Concluido = false }
            };

            var Token = TestHelper.GerarJwtFake(IDUsuario);

            var RequisicaoMontada = TipografiaHelper.FormatarRequisicao<Tarefa>(tarefasFake.AsQueryable(), Pagina, RegistrosPorPagina);

            mockTarefaService.Setup(s => s.Listar(Token, Pagina, RegistrosPorPagina, "", "", "", false))
                             .Returns(RequisicaoMontada);

            var controller = new TarefaController(mockTarefaService.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {Token}";
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = controller.ListarTarefas();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var requisicaoViewModel = Assert.IsType<RequisicaoViewModel<Tarefa>>(okResult.Value);

            Assert.Equal(2, requisicaoViewModel.Data.Count);
        }

        [Fact]
        public void SalvarTarefa_ComDadosValidos_DeveRetornarOk()
        {
            var mockTarefaService = new Mock<ITarefaService>();
            var tokenValido = TestHelper.GerarJwtFake();
            mockTarefaService.Setup(s => s.Salvar(tokenValido, It.IsAny<Tarefa>())).Returns(true);

            var controller = new TarefaController(mockTarefaService.Object);

            var tarefaFake = new Tarefa { IDUsuario = 100, Titulo = "Nova Tarefa", Conteudo = "Descrição detalhada" };

            var result = controller.SalvarTarefa(tarefaFake);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void ExcluirTarefa_DeveChamarServiceERetornarOk()
        {
            var mockTarefaService = new Mock<ITarefaService>();

            mockTarefaService.Setup(s => s.Excluir(It.IsAny<string>())).Returns(true);

            var controller = new TarefaController(mockTarefaService.Object);

            var result = controller.ExcluirTarefa("123");

            Assert.IsType<OkObjectResult>(result);
        }
    }
}