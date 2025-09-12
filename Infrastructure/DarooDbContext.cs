using Domain.Entities.Daroo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class DarooDbContext : DbContext
    {
        public DarooDbContext(DbContextOptions<DarooDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }

    }
}
