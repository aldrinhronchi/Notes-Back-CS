using Notes_Back_CS.Models.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes_Back_CS.Models.Tarefa
{
    [Table("Tarefas")]
    public class Tarefa : Entity
    {
        [Required]
        public int IDUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Column(TypeName = "text")]
        public string Conteudo { get; set; }

        [Required]
        public bool Fixado { get; set; } = false;

        [Required]
        public bool Concluido { get; set; } = false;
    }
}