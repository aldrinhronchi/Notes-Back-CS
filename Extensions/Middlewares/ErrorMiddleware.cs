using Notes_Back_CS.Connections.Database;
using Notes_Back_CS.Extensions.Helpers;
using Notes_Back_CS.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Notes_Back_CS.Extensions.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<Erro> _logger;

        public ErrorMiddleware(RequestDelegate requestDelegate, ILogger<Erro> logger)
        {
            this._requestDelegate = requestDelegate;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception exception)
            {
                ErrorInfo errorInfo = await FormatExceptionAsync(exception);
                context.Response.StatusCode = errorInfo.StatusCode;
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = null
                };

                await context.Response.WriteAsJsonAsync(errorInfo, options);
            }
        }

        public async Task<ErrorInfo> FormatExceptionAsync(Exception exception)
        {
            ErrorInfo errorInfo = new ErrorInfo();

            switch (exception)
            {
                case KeyNotFoundException ex:
                    {
                        errorInfo.StatusCode = (int)HttpStatusCode.NotFound;
                        errorInfo.Message = ex.Message;
                    }
                    break;

                case ValidationException ex:
                    {
                        errorInfo.StatusCode = (int)HttpStatusCode.BadRequest;
                        errorInfo.Message = ex.Message;
                    }
                    break;

                case UnauthorizedAccessException ex:
                    {
                        errorInfo.StatusCode = (int)HttpStatusCode.Unauthorized;
                        errorInfo.Message = ex.Message;
                    }
                    break;

                default:
                    {
                        errorInfo.StatusCode = (int)HttpStatusCode.InternalServerError;
                        try
                        {
                            string[] Aplicacao = SearchMethodNameAndClass(exception);
                            var _database = ServiceLocator.Current.BuscarServico<DatabaseContext>();
                            Erro Falha = new Erro()
                            {
                                Aplicacao = Aplicacao.Count() == 2 ? $"{Aplicacao[0]} | {Aplicacao[1]}" : "Notes | App",
                                Data = DateTime.Now,
                                Tipo = exception.GetType().ToString(),
                                Nome = exception.TargetSite != null ? exception.TargetSite.Name : string.Empty,
                                Mensagem = exception.GetFullExceptionMessage(),
                                Stack = !string.IsNullOrWhiteSpace(exception.StackTrace) ? exception.StackTrace : string.Empty,
                                Arquivo = exception.Source?.ToString(),
                            };

                            await _database.Erros.AddAsync(Falha);

                            await _database.SaveChangesAsync();

                            errorInfo.Message = $"Houve um erro, Protocolo: {Falha.ID},\n Mensagem: {exception.Message}";
                        }
                        catch (Exception ex)
                        {
                            DebugInFile(ex, "ErrorForLog");
                            errorInfo.Message = $"Houve um erro, Protocolo: 0,\n Mensagem: {exception.Message} \n Favor Contactar o suporte.";
                        }
                    }
                    break;
            }
            return errorInfo;
        }

        private static string[] SearchMethodNameAndClass(Exception ex)
        {
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                StackFrame? Frame = new StackTrace(ex).GetFrame(0);
                if (Frame != null)
                {
                    System.Reflection.MethodBase? Method = Frame.GetMethod();
                    if (Method != null)
                    {
                        string[] Array = new string[]
                        {
                            Method.GetMethodName(),
                            Method.GetClassName()
                        };
                        return Array;
                    }
                }
            }
            return new string[] { };
        }

        private void DebugInFile(Exception ex, string place)
        {
            this._logger.LogError(string.Concat("Painel Dev error at: ", DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss"), " => Erro: ", " :: ", ex.GetFullExceptionMessage()));
        }
    }
}