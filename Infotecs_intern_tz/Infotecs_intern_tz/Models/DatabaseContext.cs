using Infotecs_intern_tz.Schemas;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Infotecs_intern_tz.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ResultEntry> results { get; set; }
        public DbSet<ValueEntry> values { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueEntry>().OwnsMany(v => v.value);
            modelBuilder.Entity<ResultEntry>().OwnsOne(r => r.result);
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new DatabaseContext(
              serviceProvider.GetRequiredService<
                  DbContextOptions<DatabaseContext>>()))
            {
                DbConnection conn = context.Database.GetDbConnection();
                try
                {
                    conn.Open();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    context.Database.Migrate();
                }
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            IConfiguration configuration = builder.Build();
            string constring = configuration.GetValue<string>("ConnectionStrings:DatabaseConnection");

            optionsBuilder.UseNpgsql(constring)
             .UseLoggerFactory(LoggerFactory.Create(b => b
             .AddConsole()
             .AddFilter(level => level > LogLevel.Information)))
             .EnableSensitiveDataLogging()
             .EnableDetailedErrors();
        }
    }
}
