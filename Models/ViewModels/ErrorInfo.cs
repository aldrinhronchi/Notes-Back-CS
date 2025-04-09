namespace Notes_Back_CS.Models.ViewModels
{
    /// <summary>
    /// View Model (Sem Registros no DB) para padronizar a comunicação do Back-End com o Front-End, trazendo a exibição dos erros.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// O Codigo do Erro
        /// </summary>
        public int StatusCode { get; set; } = 0;

        /// <summary>
        /// A mensagem do Erro
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}