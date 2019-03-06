using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace WebApplication1.Contexts
{
    /*
     * Core DB context. Seems to create anything with a DbSet.
     * Somehow. 
     */
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options) : base(options)
        {
            System.Console.Out.Write("Hello");

        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Albums> Albums { get; set; }

    }

    // How do I use this ?? Won't let me inject PersonContext ??
    public class PersonRepository : IDisposable
    {
        private PersonContext db; //= new PersonContext();

        public PersonRepository()
        {
        //    this.db = db;
        }

        public IEnumerable<Person> GetAll()
        {
            return db.Persons;
        }

        public void Add(Person p)
        {
            db.Persons.Add(p);
            db.SaveChanges();
        }
        public void Dispose(bool dis)
        {
            if (dis)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
