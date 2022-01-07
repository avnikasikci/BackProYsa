
using BackProYsa.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
//using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using EntityFramework.BulkInsert.Extensions;
//using System.Data.Common;
//using System.Transactions;


namespace BackProYsa.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DataContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Tüm kayıtlar.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> All
        {
            get { return _dbSet; }
        }

        /// <summary>
        /// Kayıt bul.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T SelectById(int id)
        {
            return _dbSet.Find(id);
        }
        public virtual T SelectById(long id) //Banka hareketleri için eklendi. 
        {
            return _dbSet.Find(id);
        }
        /// <summary>
        /// Kayıt ekle.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Kayıt güncelle.
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual void Update(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        public IQueryable<T> AllNonTrackable()
        {
            return _dbSet.AsNoTracking<T>();
        }
        /// <summary>
        /// Kayıt sil.
        /// </summary>
        /// <param name="id">Kayıt id</param>
        public virtual void DeleteById(int id)
        {
            T entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Kayıt sil.
        /// </summary>
        /// <param name="entityToDelete">Kayıt</param>
        public virtual void Delete(T entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _context.Entry(entityToDelete).State = EntityState.Modified;
            _dbSet.Remove(entityToDelete);
            _context.SaveChanges();

        }
        public virtual void DeleteMultiple(IEnumerable<T> Entities)
        {
            _dbSet.RemoveRange(Entities);
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        //Update işlemi yapmaz sadece insert işlemi yapar
        public void AddMultipleInsert(IEnumerable<T> q)
        {
            _dbSet.AddRange(q);
        }

        ////Id numaralarına göre Insert ve Update
        //public void AddMultipleInsertUpdate(IEnumerable<T> q)
        //{
        //    _dbSet.AddRange(q);

        //    var itemList = new List<T>();
        //    _context.Configuration.AutoDetectChangesEnabled = false;

        //    foreach (var item in q)
        //    {
        //        itemList.Add(item);
        //        if (itemList.Count % 1000 == 0)
        //        // Play with this number to see what works best
        //        {
        //            _context.Set<T>().AddRange(itemList);
        //            itemList = new List<T>();
        //            _context.ChangeTracker.DetectChanges();
        //            _context.SaveChanges();
        //            _context?.Dispose();
        //            _context = new YourDbContext();
        //        }
        //    }

        //    _context.Set<T>().AddRange(itemList);
        //    _context.ChangeTracker.DetectChanges();
        //    _context.SaveChanges();
        //    _context?.Dispose();
        //}

        //public void BulkInsert(IEnumerable<T> entities)
        //{
        //    using (var transactionScope = new TransactionScope())
        //    {
        //        // some stuff in dbcontext

        //        //2014 beri artık bu EntityFramework.BulkInsert.Extensions package destenlenmiyor. Sorun çıkarırsa manuel yazılan BulkInsertandUpdate fonk. kullanılabilir.
        //        _context.BulkInsert(entities);

        //        _context.SaveChanges();
        //        transactionScope.Complete();
        //    }
        //}

        public TP ExecuteQueryValue<TP>(string Query, params object[] Params)//.Net Core
        {
            return (ExecuteQueryFunc<TP>(Query, false, Params)).FirstOrDefault();
        }

        public List<TP> ExecuteQuery<TP>(string Query, params object[] Params)//.Net Core
        {
            return ExecuteQueryFunc<TP>(Query, true, Params);
        }

        private List<TP> ExecuteQueryFunc<TP>(string Query, bool IsListObject, params object[] Params) //.Net core içinde yazılmış tek değer dönen veya bir obje dönen işlem.
        {
            using (var dummyCmd = _context.Database.GetDbConnection().CreateCommand())
            {
                dummyCmd.CommandText = Query;
                dummyCmd.CommandType = System.Data.CommandType.Text;

                System.Data.Common.DbParameter[] dbParameters = Params.Select(c =>
                {
                    System.Data.Common.DbParameter par = dummyCmd.CreateParameter();
                    par.ParameterName = ((System.Data.Common.DbParameter)c).ParameterName;
                    par.Value = ((System.Data.Common.DbParameter)c).Value ?? DBNull.Value;
                    return par;
                }).ToArray();

                dummyCmd.Parameters.AddRange(dbParameters);

                var entities = new List<TP>();

                _context.Database.OpenConnection();

                using (var result = dummyCmd.ExecuteReader())
                {
                    var obj = Activator.CreateInstance<TP>();
                    Type temp = typeof(TP);
                    if (!IsListObject)// int32 Decimal string vb. tek değerler için.
                    {
                        while (result.Read())
                        {
                            var val = result.IsDBNull(0) ? null : result[0];
                            entities.Add((TP)val);
                            break;
                        }
                    }
                    else
                    {
                        while (result.Read())
                        {

                            var newObject = Activator.CreateInstance<TP>();
                            for (var i = 0; i < result.FieldCount; i++)
                            {
                                var name = result.GetName(i);

                                PropertyInfo prop = temp.GetProperties().ToList().FirstOrDefault(a => a.Name.ToLower().Equals(name.ToLower()));

                                if (prop == null)
                                {
                                    continue;
                                }
                                var val = result.IsDBNull(i) ? null : result[i];
                                prop.SetValue(newObject, val, null); // Gönderilen T objesi nullable ise hata alınabilir. 
                            }
                            //entities.Add(map(result));
                            entities.Add(newObject);
                        }
                    }
                }

                return entities;
            }

        }

        public List<TP> RawQuery<TP>(string Query, params object[] Params)
        {

            var QuerySql = (IQueryable<TP>)_dbSet.FromSqlRaw(Query, Params ?? new object[0]);
            return QuerySql.ToList();
        }

        public IQueryable<T> RawQuery(string Query, params object[] Parameters)// context içine nesne DbSet edilmiş ise cağırılır DTO dahil.
        {
            //return _context.Database.SqlQuery<T>(Query, Parameters);
            //return _context.Database.< T > (Query, Parameters);

            return (IQueryable<T>)_dbSet.FromSqlRaw(Query, Parameters ?? new object[0]);
            //return QuerySql.AsEnumerable();
        }
        public IEnumerable<TP> RawQuery<TP>(string Query, params Tuple<string, object>[] Parameters)
        {
            var dummyCmd = _context.Database.GetDbConnection().CreateCommand();


            var dbParameters = Parameters.Select(c =>
            {
                var par = dummyCmd.CreateParameter();
                par.ParameterName = c.Item1;
                par.Value = c.Item2 ?? DBNull.Value;
                return par;
            }).ToArray();
            //return _context.Database.ExecuteSqlRaw<T>(Query, Parameters);

            var QuerySql = (IQueryable<TP>)_dbSet.FromSqlRaw(Query, Parameters); // Net Core İçin Eklendi
            return QuerySql.AsEnumerable();

            ////ORNEK
            //        var result = _StokGroupRepository.RawQuery<StokGroupUDFDTO>("Udf_GetStokGroupDetail",
            //             new Tuple<string, object>[]{
            //                new Tuple<string, object>("CountryCode", "TR"),
            //                new Tuple<string, object>("LanguageCode", "TUR"),
            //                new Tuple<string, object>("GroupCodes", string.Join(",",groupCodes)),
            //});
            //        return result;
        }

        public void ExecuteRawQuery(string Query, params object[] Parameters)
        {
            _context.Database.ExecuteSqlRaw(Query, Parameters ?? new object[0]);
            //_context.Database.ExecuteSqlCommand(Query, Parameters??new object[0]);
        }
        public void ExecuteRawQueryDoNotEnsureTransaction(string Query, params object[] Parameters)
        {
            _context.Database.ExecuteSqlRawAsync(Query, Parameters ?? new object[0]);
            //_context.Database.ExecuteSqlRaw(TransactionalBehavior.DoNotEnsureTransaction, Query, Parameters ?? new object[0]);

            //_context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, Query, Parameters ?? new object[0]);
        }

        #region //For Bulk Update
        public string ConnectionStr()
        {
            //return _context.Database.Connection.ConnectionString;
            return _context.Database.GetConnectionString();

        }
        public string GetTableName()
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            //var schema = entityType.GetSchema();
            //var schemaAnnotation = entityType.GetAnnotations().FirstOrDefault(a => a.Name == "Relational:Schema");
            //var schema = schemaAnnotation == null ? "dbo" : schemaAnnotation.Value.ToString();
            var tableName = entityType.GetTableName();
            return tableName;

            //var objectContext = ((Entity.Infrastructure.IObjectContextAdapter)_context).ObjectContext;

            //string sql = objectContext.CreateObjectSet<T>().ToTraceString();
            //var regex = new System.Text.RegularExpressions.Regex("FROM (?<table>.*) AS");
            //var match = regex.Match(sql);

            //string table = match.Groups["table"].Value;
            //return table;
        }
        public string GetTableNameWithShema()
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var schemaAnnotation = entityType.GetAnnotations().FirstOrDefault(a => a.Name == "Relational:Schema");
            var schema = schemaAnnotation == null ? "dbo" : schemaAnnotation.Value.ToString();
            var tableName = entityType.GetTableName();
            return $"{schema}.[{tableName}]";
        }
        #endregion //For Bulk Update

        //public List<T> SqlQuery<T>(string Query)
        //{
        //    return _context.Database.SqlQuery<T>(Query).ToList();
        //}

        public void SetConnectionTimeout(int ConnectionTimeout)
        {
            _context.Database.SetCommandTimeout(ConnectionTimeout); // Default 30 yani 30sn
        }

    }

}
