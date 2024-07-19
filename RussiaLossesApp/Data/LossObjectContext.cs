using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RussiaLossesApp.Models;

namespace RussiaLossesApp.Data
{
    public class LossObjectContext : DbContext
    {
        public LossObjectContext (DbContextOptions<LossObjectContext> options)
            : base(options)
        {
        }

        public DbSet<RussiaLossesApp.Models.LossObject> LossObject { get; set; } = default!;
        public DbSet<RussiaLossesApp.Models.EquipType> EquipType { get; set;} = default!;
        public DbSet<RussiaLossesApp.Models.EquipCategory> EquipCategory { get; set; } = default!;
    }
}
