using BankingAppFinal.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppFinal.Services.Services
{
    public class BankingAppDbContext : DbContext
    {
        public BankingAppDbContext() : base("BankingAppDbContext")
        {
        }

        public DbSet<Account> AccountSet { get; set; }

        public DbSet<UserAccount> UserAccountSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<BankingAppDbContext>(null);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>()
                       .Property(p => p.Balance)
                       .HasPrecision(10, 5); // or whatever your schema specifies
        }

        public void BulkInsertAll<T>(IEnumerable<T> entities)
        {
            
            using (var conn = new SqlConnection(Database.Connection.ConnectionString))
            {
                conn.Open();

                Type t = typeof(T);


                using (SqlTransaction transaction = conn.BeginTransaction())
                {

                    SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, transaction)
                    {
                        DestinationTableName = GetTableName(t)
                    };

                    var table = new DataTable();

                    var properties = t.GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string));

                    foreach (var property in properties)
                    {
                        Type propertyType = property.PropertyType;
                        if (propertyType.IsGenericType &&
                            propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            propertyType = Nullable.GetUnderlyingType(propertyType);
                        }

                        table.Columns.Add(new DataColumn(property.Name, propertyType));

                        if (property.Name != "Id")
                        {
                            bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(property.Name, property.Name));
                        }
                    }

                    foreach (var entity in entities)
                    {
                        table.Rows.Add(properties.Select(property => property.GetValue(entity, null) ?? DBNull.Value).ToArray());
                    }

                    if (table.Columns.Contains("Id"))
                        table.Columns.Remove("Id");

                  

                    bulkCopy.BatchSize = 1000;
                    //bulkCopy.NotifyAfter = 1000;
                    //bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                    bulkCopy.WriteToServer(table);
                    transaction.Commit();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Getting the table name from the database object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetTableName(Type type)
        {
            var metadata = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            return (table.Schema + "." + "[" + (string)table.MetadataProperties["Table"].Value + "]") ?? ("dbo." + table.Name);
        }
    }
}
