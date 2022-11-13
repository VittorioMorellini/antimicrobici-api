using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Antimicrobici.Core.Utils
{
    public interface IBaseService<TEntity, TKey, TContext>
    {
        TEntity Find(TKey id);
        TEntity Save(TKey id, TEntity item);
        Task<TEntity> SaveAsync(TKey id, TEntity item);

        // TODO sono da testare
        TEntity Save(TEntity item, bool includeGraph = false);
        void Save(IEnumerable<TEntity> entities, bool includeGraph = false);

        bool Delete(TKey id);
        
        EntityState GetEntityState(TKey id, TEntity item);
        TContext GetContext();

        void BulkInsert(TEntity item);
        void BulkInsert(IEnumerable<TEntity> entities);
        void BulkUpdate(TEntity item);
        void BulkUpdate(IEnumerable<TEntity> entities);
        void BulkDelete(TEntity item);
        void BulkDelete(IEnumerable<TEntity> entities);
    }

    public interface IBaseService<TEntity>
    {
        TEntity Find(long id);
        TEntity Save(long id, TEntity item);
        bool Delete(long id);        
    }

    public class BaseService<TEntity, TKey, TContext> 
        where TEntity : class
        where TContext : DbContext
    {
        protected TContext ctx;
        protected string userId;

        public BaseService(TContext ctx)
        {
            this.ctx = ctx;
        }

        public T WithUserIdSession<T>(Func<T> func)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return func.Invoke();
            }
            else
            {
                var connection = ctx.Database.GetDbConnection();
                try
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"exec sp_set_session_context @key=N'user_id', @value=@UserId";
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));
                    cmd.ExecuteNonQuery();

                    return func.Invoke();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual TEntity Find(TKey id)
        {
            return WithUserIdSession(() => ctx.Set<TEntity>().Find(id));
        }

        public virtual bool Delete(TKey id)
        {
            return WithUserIdSession(() => ctx.Delete<TEntity, TKey>(id));
        }

        public virtual TEntity Save(TKey id, TEntity item)
        {
            return WithUserIdSession(() => ctx.Save(id, item));
        }

        public virtual Task<TEntity> SaveAsync(TKey id, TEntity item)
        {
            return ctx.SaveAsync(id, item);
        }

        public virtual TEntity Save(TEntity item, bool includeGraph = false)
        {
            return WithUserIdSession(() =>
            {
                // IncludeGraph permette il salvataggio anche dell'entità correlate (passate) dell'entity
                ctx.BulkMerge(new TEntity[] { item }, options => options.IncludeGraph = includeGraph);
                // non so se item contiene l'identity nel caso di insert
                return item;
            });
        }

        public virtual void Save(IEnumerable<TEntity> entities, bool includeGraph = false)
        {
            ctx.Save(includeGraph, entities);
        }

        public EntityState GetEntityState(TKey id, TEntity item)
        {
            return ctx.GetEntityState(id, item);
        }

        public TContext GetContext()
        {
            return ctx;
        }

        public void BulkInsert(TEntity item)
        {
            BulkInsert(new TEntity[] { item });
        }

        public void BulkInsert(IEnumerable<TEntity> entities)
        {
            ctx.BulkInsert(entities);
        }

        public void BulkUpdate(IEnumerable<TEntity> entities)
        {
            ctx.BulkUpdate(entities);
        }

        public void BulkUpdate(TEntity item)
        {
            BulkUpdate(new TEntity[] { item });
        }

        public void BulkDelete(TEntity item)
        {
            BulkDelete(new TEntity[] { item });
        }

        public void BulkDelete(IEnumerable<TEntity> entities)
        {
            ctx.BulkInsert(entities);
        }
    }
}
