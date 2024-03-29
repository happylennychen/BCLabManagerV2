﻿using BCLabManager.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public AppDbContext _context = null;
        public DbSet<T> table = null;
        public Repository()
        {
            this._context = new AppDbContext();
            this.table = _context.Set<T>();
        }
        public Repository(AppDbContext dbContext)
        {
            this._context = dbContext;
            this.table = _context.Set<T>();
        }
        public void Delete(object id)
        {
            T obj = table.Find(id);
            Delete(obj);
        }

        public void Delete(T obj)
        {
            if (_context.Entry(obj).State == EntityState.Detached)
            {
                table.Attach(obj);
            }
            table.Remove(obj);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return table.Where(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }

        public IEnumerable<T> GetAll(string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = table;

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    )
                    query = query.Include(includeProperty);
                return query.ToList();
            }
            catch (Exception e)
            {
                //Event evt = new Event();
                //evt.Module = Module.Database;
                //evt.Type = EventType.Error;
                //evt.Timestamp = DateTime.Now;
                //evt.Description = $"Cannot access database. Details:\n" +
                //    $"\tMessage: {e.Message}\n" +
                //    $"\tInnerException: {e.InnerException}";
                //EventService.SuperAdd(evt);
                throw e;
            }
        }

        public T GetById(object id)
        {
            return table.Find(id);
        }

        public void Insert(T obj)
        {
            table.Add(obj);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return table.SingleOrDefault(predicate);
        }

        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
    }
}
