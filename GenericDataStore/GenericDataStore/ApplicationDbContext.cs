using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GenericDataStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GenericDataStore
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions option)
    : base(option)
        {
            Database.SetCommandTimeout(30000);
        }
        public DbSet<ObjectType> ObjectType { get; set; }

        public DbSet<Field> Field { get; set; }

        public DbSet<Option> Option { get; set; }

        public DbSet<UserMessage> UserMessage { get; set; }

        public DbSet<Offer> Offer { get; set; }

        public DbSet<DatabaseTableRelations> DatabaseTableRelations { get; set; }


        public DbSet<DatabaseConnectionProperty> DatabaseConnectionProperty { get; set; }

        public DbSet<TablePage> TablePage { get; set; }




    }
}
