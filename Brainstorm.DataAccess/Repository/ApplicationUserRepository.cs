using Brainstorm.DataAccess.Data;
using Brainstorm.DataAccess.Repository.IRepository;
using Brainstorm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository //thừa kế từ lớp Repository<Category> và triển khai giao diện ICategoryRepository.
    {
        private readonly ApplicationDbContext _db;//biến này chỉ được đọc(không được ghi hay làm gì khác).

        public ApplicationUserRepository(ApplicationDbContext db) : base(db) //gọi đến constructor của lớp cha Repository<T> để khởi tạo DbSet.
        {
            _db = db;// gán giá trị cho biến _db.
        }



    }
}
