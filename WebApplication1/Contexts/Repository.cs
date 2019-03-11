using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Contexts
{
    public interface IRepository<Thing> where Thing : class
    {
        IEnumerable<Thing> GetAll();
        void Insert(Thing ent);
    }
    public class Repository<Entity> : IRepository<Entity> where Entity : class
    {
        internal DbContext context;
        internal DbSet<Entity> ents;

        public Repository() { }

        public Repository(DbContext pc)
        {
            this.context = pc;
            ents = this.context.Set<Entity>();
        }

        public virtual IEnumerable<Entity> GetAll()
        {
            return context.Set<Entity>().AsEnumerable();
        }

        public virtual void Insert(Entity ent) {
            //var ret = context.Set<Entity>().Add(ent);
            var ret = ents.Add(ent);
        }
    }
}
