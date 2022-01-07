using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Tüm kayıtlar.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> All { get; }
        IQueryable<T> AllNonTrackable();
        /// <summary>
        /// Kayıt bul.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T SelectById(int id);
        T SelectById(long id);

        /// <summary>
        /// Kayıt ekle.
        /// </summary>
        /// <param name="entity"></param>
        void Insert(T entity);

        /// <summary>
        /// Kayıt güncelle.
        /// </summary>
        /// <param name="entityToUpdate"></param>
        void Update(T entityToUpdate);

        /// <summary>
        /// Kayıt sil.
        /// </summary>
        /// <param name="id">Kayıt id</param>
        void DeleteById(int id);

        /// <summary>
        /// Kayıt sil.
        /// </summary>
        /// <param name="entityToDelete">Kayıt</param>
        void Delete(T entityToDelete);
        void DeleteMultiple(IEnumerable<T> Entities);

        void SaveChanges();
        void AddMultipleInsert(IEnumerable<T> q);

        TP ExecuteQueryValue<TP>(string Query, params object[] Params);//.net Core Destekli
        List<TP> ExecuteQuery<TP>(string Query, params object[] Params); //.net Core Destekli

        void ExecuteRawQuery(string Query, params object[] Parameters);
        void ExecuteRawQueryDoNotEnsureTransaction(string Query, params object[] Parameters);
        List<TP> RawQuery<TP>(string Query, params object[] Params);
        IQueryable<T> RawQuery(string Query, params object[] Parameters);// context içine nesne DbSet edilmiş ise cağırılır DTO dahil.

        string ConnectionStr();
        string GetTableName();
        string GetTableNameWithShema();

        void SetConnectionTimeout(int ConnectionTimeout);
    }

}
