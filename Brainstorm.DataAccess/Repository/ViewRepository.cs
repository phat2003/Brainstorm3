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
    public class ViewRepository : Repository<View>, IViewRepository
    {
        private readonly ApplicationDbContext _db;
        public ViewRepository(ApplicationDbContext db) : base(db) //gọi đến constructor của lớp cha Repository<T> để khởi tạo DbSet.
        {
            _db = db;// gán giá trị cho biến _db.
        }


        int IViewRepository.IncrementCount(View view, int count)
        {
            view.VisitTime += count;//tăng số lượng sản phẩm trong giỏ hàng lên count.
            return view.VisitTime;//trả về số lượng sản phẩm trong giỏ hàng sau khi đã tăng.
        }


    }
}
