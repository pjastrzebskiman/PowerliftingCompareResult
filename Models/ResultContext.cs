using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace PowerliftingCompareResult.Models
{

    public class ResultContext :DbContext
    {
    public ResultContext(DbContextOptions<ResultContext> options): base(options)
        {

        }
        public  DbSet <LiftResult> LiftResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LiftResult>().HasNoKey();
        }

     

    }
}
