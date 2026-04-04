using Brainstorm.DataAccess.Data;
using Brainstorm.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;//biến này chỉ được đọc(không được ghi hay làm gì khác).
        internal DbSet<T> DbSet;//DbSet là một tập hợp các đối tượng của một loại cụ thể trong Entity Framework.
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.DbSet = _db.Set<T>();//DbSet được khởi tạo bằng cách gọi phương thức Set<T>() của ApplicationDbContext.
        }
        public void Add(T entity)
        {
            DbSet.Add(entity); //thêm đối tượng vào DbSet.
        }

        // include category,covertype
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;//khởi tạo một truy vấn IQueryable từ DbSet.

            if (filter != null)
            {
                query = query.Where(filter);//lọc các đối tượng trong DbSet theo điều kiện được cung cấp trong filter.
            }
            
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);//nếu includeProperties khác null thì truy vấn sẽ bao gồm các thuộc tính liên quan được chỉ định trong includeProperties. 
                }
                
            }
            return query.ToList(); //trả về danh sách tất cả các đối tượng trong DbSet.
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;//khởi tạo một truy vấn IQueryable từ DbSet.
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);//nếu includeProperties khác null thì truy vấn sẽ bao gồm các thuộc tính liên quan được chỉ định trong includeProperties. 
                }

            }
            query = query.Where(filter);//lọc các đối tượng trong DbSet theo điều kiện được cung cấp trong filter.
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity); //xóa đối tượng khỏi DbSet. (ở đây là xoá 1)
        }

        public void removeRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities); //xóa một tập hợp các đối tượng khỏi DbSet. (ở đây là xoá nhiều)
        }
    }
}
