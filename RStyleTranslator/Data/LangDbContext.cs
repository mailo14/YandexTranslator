using RStyleTranslator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace RStyleTranslator
{
    public class LangDbContext : DbContext
    {
        public LangDbContext() : base("name=LangDbConnection") { }
        public DbSet<Lang> Langs { get; set; }
        public DbSet<Route> Routes { get; set; }        
    }
}
