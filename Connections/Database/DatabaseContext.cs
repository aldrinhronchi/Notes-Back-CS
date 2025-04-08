using Microsoft.EntityFrameworkCore;
using Notes_Back_CS.Models.Entities;

namespace Notes_Back_CS.Connections.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Erro> Erros { get; set; }
    }
}