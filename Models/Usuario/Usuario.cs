using Notes_Back_CS.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes_Back_CS.Models.Usuario
{
    [Table("Usuarios")]
    public class Usuario : Entity
    {
        [Required]
        [StringLength(100)]
        public String Nome { get; set; }

        [Required]
        [StringLength(100)]
        public String Login { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(100)]
        public String Senha { get; set; }

        [Required]
        [StringLength(200)]
        public String Email { get; set; }

        [NotMapped]
        public String? Token { get; set; }
    }
}