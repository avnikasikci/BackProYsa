using BackProYsa.DataAccess.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            //Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<NeuralNodes> NeuralNodes { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<NeuralBpLayer> NeuralBpLayer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
