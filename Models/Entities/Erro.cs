using System.ComponentModel.DataAnnotations.Schema;

namespace Notes_Back_CS.Models.Entities
{
    /// <summary>
    /// Classe para fazer Log dos erros no DB para uma visualização remota
    /// </summary>
    [Table("ErrosLog")]
    public class Erro
    {
        /// <summary>
        /// Identificador
        /// </summary>
        public Int32 ID { get; set; }

        /// <summary>
        /// Aplicação que causou o erro sendo salva no template "NomeFunção | NomeClasse"
        /// </summary>
        public String? Aplicacao { get; set; }

        /// <summary>
        /// Metodo que gerou o erro
        /// </summary>
        public String? Nome { get; set; }

        /// <summary>
        /// Data de erro
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Tipo da Exceção
        /// </summary>
        public String? Tipo { get; set; }

        /// <summary>
        /// Mensagem da Exception
        /// </summary>
        public String? Mensagem { get; set; }

        /// <summary>
        /// Arquivo que gerou a Exception
        /// </summary>
        public String? Arquivo { get; set; }

        /// <summary>
        /// Stack Trace da Exception
        /// </summary>
        public String? Stack { get; set; }
    }
}