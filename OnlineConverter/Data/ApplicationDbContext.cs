﻿using Microsoft.EntityFrameworkCore;
using OnlineConverter.Models;

namespace OnlineConverter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Currency> Currencies { get; set; }
    }
}