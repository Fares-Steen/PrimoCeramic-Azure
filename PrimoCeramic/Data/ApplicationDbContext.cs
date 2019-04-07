using System;
using System.Collections.Generic;
using System.Text;
using PrimoCeramic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PrimoCeramic.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductTypes> ProductTypes { get; set; }

        public DbSet<SpecialTags> SpecialTags { get; set; }





        public DbSet<Products> Products { get; set; }

        public DbSet<Appointments> Appointments { get; set; } 

        public DbSet<ProductsSelectedForAppointment> ProductsSelectedForAppointment { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<ProductsSelectedForOrder> ProductsSelectedForOrder { get; set; }

        public DbSet<CustomerShoppingCart> CustomerShoppingCart { get; set; }

        public DbSet<ProductImages> ProductImages { get; set; }

        public DbSet<ProductCountry> ProductCountry { get; set; }

        public DbSet<ProductApplication> ProductApplication { get; set; }

        public DbSet<ProductSurface> ProductSurface { get; set; }

        public DbSet<Emails> Emails { get; set; }
    
        public DbSet<Statics> Statics { get; set; }
    }
}
