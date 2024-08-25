﻿using Microsoft.EntityFrameworkCore;
using MyShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context; 
            _dbSet =  _context.Set<T>();    
        }
        public void Add(T entity)
        {
           _dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
             _dbSet?.Remove(entity);    
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate= null, string? includeword = null)
        {
            IQueryable<T> query = _dbSet;
            if(predicate != null)
            {
                query = query.Where(predicate);
            }
            if(includeword != null)
            {
                //context.Products.Include("Category,logos, users");
                foreach(var item in includeword.Split(new Char[] {','}, StringSplitOptions.RemoveEmptyEntries)) 
                {
                    query = query.Include(item);
                }
            }

            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate, string includeword)
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeword != null)
            {
                foreach (var item in includeword.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            return query.SingleOrDefault();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
 
    }
}
