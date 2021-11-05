using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Archi.API.Models;
using System.Threading;
using Archi.Librari.Models;

namespace Archi.API.Data
{
    public class ArchiDbContext : DbContext
    {
        public ArchiDbContext(DbContextOptions options) :base(options)
        {
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeAddedState();
            ChangeDeleteState();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeAddedState();
            ChangeDeleteState();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void ChangeAddedState()
        {
            var entites = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
            foreach (var item in entites)
            {
                item.State = EntityState.Added;
                if (item.Entity is ModelBase)
                {
                    ((ModelBase)item.Entity).Active = true;
                }
            }
        }
        private void ChangeDeleteState()
        {
            var entites = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);
            foreach (var item in entites)
            {
                item.State = EntityState.Modified;
                if (item.Entity is ModelBase)
                {
                    ((ModelBase)item.Entity).Active = false;
                }
            }
        }

        //<> typage generique
        public DbSet<Customer> Customers { get; set; }
        //Obligatoire pour faire la migration
        public DbSet<Pizza> Pizzas { get; set; }

    }
}
