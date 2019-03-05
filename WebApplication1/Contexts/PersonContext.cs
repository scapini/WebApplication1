using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace WebApplication1.Contexts
{
    public class PersonContext : DbContext
    {
        public PersonContext()
        {
        }
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=mydb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }

    public class PersonRepository : IDisposable
    {
        private PersonContext db = new PersonContext();

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
