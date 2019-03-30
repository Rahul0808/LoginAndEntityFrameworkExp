using BankingAppFinal.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppFinal.Services.Services
{
    public class BaseCRUDService<T> : BaseService where T : BaseEntity
    {
        /// <summary>
        /// Method to Insert a record in T Class
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Record after database entry</returns>
        public T Create(T entity)
        {
            try
            {
                using (var db = CreateDbContext())
                {
                    entity = GetDbSet<T>(db).Add(entity);
                    db.SaveChanges();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when creating {typeof(T).Name}.", ex);
            }
        }

        /// <summary>
        /// Method to insert multiple record ar once in database
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>List of records after insertion</returns>
        public IList<T> CreateMultiple(IList<T> entities)
        {
            try
            {
                using (var db = CreateDbContext())
                {
                    (db as BankingAppDbContext).BulkInsertAll(entities);
                    return entities;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when creating {typeof(T).Name}.", ex);
            }
        }

        /// <summary>
        /// Method to update a record in T entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Records after update in database</returns>
        public T Update(T entity)
        {
            try
            {
                using (var db = CreateDbContext())
                {
                    var entityInDb = CheckIfEntityExist(db, entity.Id);
                    var entry = db.Entry(entityInDb);

                    entry.CurrentValues.SetValues(entity);
                    db.SaveChanges();

                    return entry.Entity;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when updating {typeof(T).Name}.", ex);
            }
        }

        /// <summary>
        /// Get records from database corresponding to an id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Get record corresponding to an id</returns>
        public T Get(long id)
        {
            try
            {
                using (var db = CreateDbContext())
                {
                    return CheckIfEntityExist(db, id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when getting {typeof(T).Name} with id:{id}.", ex);
            }
        }

        /// <summary>
        /// Method to return list of all records in  a table
        /// </summary>
        /// <returns>List of all records in a table</returns>
        public IList<T> GetAll()
        {
            try
            {
                using (var db = CreateDbContext())
                {
                    IQueryable<T> query = GetDbSet<T>(db);
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when getting all {typeof(T).Name} entities.", ex);
            }
        }

        /// <summary>
        /// Delete a record in a table corresponding to to an id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(long id)
        {
            try
            {
                using (var db = CreateDbContext())
                {
                    T entity = CheckIfEntityExist(db, id);
                    GetDbSet<T>(db).Remove(entity);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred when deleting {typeof(T).Name} with id:{id}.", ex);
            }
        }

        /// <summary>
        /// Check wwhetheer a record exist in a table
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns>Returns a record corresponding to a table</returns>
        protected T CheckIfEntityExist(DbContext db, long id)
        {
            IQueryable<T> query = GetDbSet<T>(db);
            var entityInDb = query.FirstOrDefault(i => i.Id == id);
            if (entityInDb == null)
            {
                throw new Exception($"{typeof(T).Name} does not exists with Id={id}.");
            }

            return entityInDb;
        }
    }
}
