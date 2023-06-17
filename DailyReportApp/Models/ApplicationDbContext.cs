using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DailyReportApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                InitialCatalog = "DailyReportDB",
                IntegratedSecurity = true
            };

            //optionsBuilder.UseSqlServer(connectionStringBuilder.ConnectionString);
            optionsBuilder.UseSqlServer(@"Data Source=172.16.6.11; Initial Catalog=DailyReportData01; User ID=sa;Password=Sapassword1; Encrypt=false");

        }
    }
}
