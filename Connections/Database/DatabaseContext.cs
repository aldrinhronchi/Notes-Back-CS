using Microsoft.EntityFrameworkCore;
using Notes_Back_CS.Models.Entities;
using Notes_Back_CS.Models.Tarefa;
using Notes_Back_CS.Models.Usuario;

namespace Notes_Back_CS.Connections.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Erro> Erros { get; set; }

        #region Usuario

        public DbSet<Usuario> Usuarios { get; set; }

        #endregion Usuario

        #region Tarefa

        public DbSet<Tarefa> Tarefas { get; set; }

        #endregion Tarefa
    }
}