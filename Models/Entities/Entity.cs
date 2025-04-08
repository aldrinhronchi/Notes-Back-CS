using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Notes_Back_CS.Models.Entities
{
    /// <summary>
    /// Classe Pai contendo o basico para boa parte dos cadastros <br/>
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Identificador Unico, de Um em Um
        /// </summary>
        [Required]
        public Int32 ID { get; set; }

        /// <summary>
        /// Registro Ativo ou Inativo
        /// </summary>
        [Required]
        public Boolean Ativo { get; set; } = true;

        /// <summary>
        /// Data de Criação, tendo valor padrão de GetDate()
        /// </summary>
        [Required]
        public DateTime DataCriado { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da Última Alteração
        /// </summary>
        public DateTime? DataAlterado { get; set; }
    }
}