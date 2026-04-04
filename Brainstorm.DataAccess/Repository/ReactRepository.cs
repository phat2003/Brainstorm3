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
    public class ReactRepository : Repository<React>, IReactRepository
    {
        private readonly ApplicationDbContext _db;
        public ReactRepository(ApplicationDbContext db) : base(db) //gọi đến constructor của lớp cha Repository<T> để khởi tạo DbSet.
        {
            _db = db;// gán giá trị cho biến _db.
        }


        int IReactRepository.IncrementCount(React react, int count)
        {
            react.ReactValue += count;//tăng số lượng sản phẩm trong giỏ hàng lên count.
            return react.ReactValue;//trả về số lượng sản phẩm trong giỏ hàng sau khi đã tăng.
        }

        int IReactRepository.DecrementCount(React react, int count)
        {
            react.ReactValue -= count;//giảm số lượng sản phẩm trong giỏ hàng đi count.
            return react.ReactValue;//trả về số lượng sản phẩm còn lại trong giỏ hàng sau khi đã giảm.
        }

        public void Update(React obj)
        {
            // Báo cho Entity Framework biết để cập nhật đối tượng này
            _db.Reacts.Update(obj);
        }

    }
}
