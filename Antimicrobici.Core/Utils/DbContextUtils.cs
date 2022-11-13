using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.Data.SqlClient;
//using Z.EntityFramework.Extensions;

namespace Antimicrobici.Core.Utils
{
    public static class DbContextUtils
    {
        static DbContextUtils()
        {
            //Z.EntityFramework.Extensions.LicenseManager.AddLicense("2907;101-Sixtema", "da204260-a12d-2135-7628-1ad431e513f8");
            //if (!Z.EntityFramework.Extensions.LicenseManager.ValidateLicense(out string licenseErrorMessage))
            //    throw new Exception(licenseErrorMessage);
        }

        [Obsolete]
        public static void AddOrUpdate<TEntity, TKey>(this DbContext context, TKey id, TEntity entity)
            where TEntity : class
        {
            var i = context.Set<TEntity>().Find(id);
            if (i != null)
            {
                context.Entry(i).State = EntityState.Detached;

                context.Set<TEntity>().Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                context.Set<TEntity>().Add(entity);
                var e = context.Entry(entity);
            }
        }

        [Obsolete]
        public static TEntity Save<TEntity, TKey>(this DbContext context, TKey id, TEntity entity)
            where TEntity : class
        {
            context.AddOrUpdate(id, entity);
            context.SaveChanges();
            return entity;
        }

        //public static void Save<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        //{
        //    context.BulkMerge(new TEntity[] { entity });
        //}

        //public static void Save<TEntity>(this DbContext context, IEnumerable<TEntity> entities, bool includeGraph = false) where TEntity : class
        //{
        //    context.BulkMerge(entities, options => options.IncludeGraph = includeGraph);
        //}

        [Obsolete]
        public static async Task<TEntity> SaveAsync<TEntity, TKey>(this DbContext context, TKey id, TEntity entity)
            where TEntity : class
        {
            context.AddOrUpdate(id, entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public static EntityState GetEntityState<TEntity, TKey>(this DbContext context, TKey id, TEntity entity)
            where TEntity : class
        {
            return context.Set<TEntity>().Find(id) != null ? EntityState.Modified : EntityState.Added;
        }

        public static bool Delete<TEntity, TKey>(this DbContext context, TKey id)
            where TEntity : class
        {
            bool status = false;

            var i = context.Set<TEntity>().Find(id);
            if (i != null)
            {
                context.Entry(i).State = EntityState.Deleted;
                context.SaveChanges();

                status = true;
            }

            return status;
        }
    }

    public static class DbDataReaderUtils
    {
        public static string GetNullableString(this DbDataReader reader, int i)
        {
            object value = reader.GetValue(i);
            return value == DBNull.Value ? null : (string)value;
        }

        public static DateTime? GetNullableDateTime(this DbDataReader reader, int i)
        {
            object value = reader.GetValue(i);
            return value == DBNull.Value ? null : (DateTime?)value;
        }

        public static short? GetNullableInt16(this DbDataReader reader, int i)
        {
            object value = reader.GetValue(i);
            return value == DBNull.Value ? null : (short?)value;
        }

        public static int? GetNullableInt32(this DbDataReader reader, int i)
        {
            object value = reader.GetValue(i);
            return value == DBNull.Value ? null : (int?)value;
        }

        public static long? GetNullableInt64(this DbDataReader reader, int i)
        {
            object value = reader.GetValue(i);
            return value == DBNull.Value ? null : (long?)value;
        }

        public static bool? GetNullableBoolean(this DbDataReader reader, int i)
        {
            object value = reader.GetValue(i);
            return value == DBNull.Value ? null : (bool?)value;
        }

        public static void AddNullable(this DbParameterCollection parameters, string parName, object parValue)
        {
            if (parValue != null)
                parameters.Add(new SqlParameter(parName, parValue));
            else
                parameters.Add(new SqlParameter(parName, DBNull.Value));
        }
    }

    public class ServiceResponse<T>
    {
        public T Result { get; set; }
        public Exception Exception { get; set; }
        public bool Error { get { return Exception != null; } }
    }
}
