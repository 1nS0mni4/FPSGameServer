using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DB {
    public class AppDbContext : DbContext{
        public AppDbContext() {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            //DbPath = System.IO.Path.Join(path, "FPSOnline.db");
        }

        #region Contents
        public DbSet<UserAccount> UserAccounts { get; set; }

        #endregion

        public const string connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FPSGame;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        protected override void OnConfiguring(DbContextOptionsBuilder options) {
            options.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<UserAccount>().HasKey(i => i.UserAccountID).IsClustered();
            builder.Entity<UserAccount>().HasIndex(i => i.ID).IsUnique();

            builder.Entity<UserAccount>().Property("AuthCode")
                .HasDefaultValue(-1);

        }
    }
}
