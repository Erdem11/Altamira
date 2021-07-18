using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altamira.Repository;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Altamira.EfRepository
{
    public class EfRepositoryBase<T> : IRepositoryBase<T> where T : class, new()
    {
        public EfRepositoryBase(DbContextOptions<EfContext> options)
        {
            EfContext = new EfContext(options);
        }

        protected readonly EfContext EfContext;

        public async Task<T> Add(T entity)
        {
            await EfContext.Set<T>().AddAsync(entity);
            await EfContext.SaveChangesAsync();

            return entity;
        }

        public async Task<T> Get(int id)
        {
            return await EfContext.Set<T>().FindAsync(id);
        }

        public async Task<T> Delete(int id)
        {
            var dbSet = EfContext.Set<T>();

            var entity = await dbSet.FindAsync(id);
            dbSet.Remove(entity);

            await EfContext.SaveChangesAsync();
            return entity;
        }

        public async Task<OneOf<T, Exception>> Update(int id, T entity)
        {
            var dbEntity = await EfContext.Set<T>().FindAsync(id);
            if (dbEntity == default)
            {
                return new Exception("Not Found");
            }

            EfContext.Entry(dbEntity).State = EntityState.Detached;
            EfContext.Entry(entity).State = EntityState.Modified;

            await EfContext.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<T>> Get()
        {
            return await EfContext.Set<T>().ToListAsync();
        }
    }
}