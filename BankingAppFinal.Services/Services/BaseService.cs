using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppFinal.Services.Services
{
    public class BaseService
    {
        /// <summary>
        /// Creating a database context in Database
        /// </summary>
        /// <returns></returns>
        protected DbContext CreateDbContext()
        {
            var db = new BankingAppDbContext();
            db.Configuration.LazyLoadingEnabled = false;
            return db;
        }

        /// <summary>
        /// Get Database records in a set for a table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DbSet<T> GetDbSet<T>(DbContext db) where T : class
        {
            var propertyName = typeof(T).Name + "Set";
            var property = db.GetType().GetProperty(propertyName, typeof(DbSet<T>));
            if (property == null)
            {
                throw new Exception($"Property {propertyName} of type {typeof(DbSet<T>).Name} can't be found on {db.GetType().Name}");
            }
            return (DbSet<T>)property.GetValue(db);
        }
    }

}
