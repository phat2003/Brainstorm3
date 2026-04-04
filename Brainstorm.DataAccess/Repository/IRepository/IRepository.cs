using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class// ràng buộc T phải là một lớp (class).
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);//biểu thức lambda để lọc các đối tượng trong DbSet.
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);//trả về danh sách tất cả các đối tượng trong DbSet.
        void Add(T entity);//thêm 1 đối tượng vào DbSet.
        void Remove(T entity);//xóa 1 đối tượng khỏi DbSet.
        void removeRange(IEnumerable<T> entities);//xóa một tập hợp nhiều các đối tượng khỏi DbSet.
    }
}
