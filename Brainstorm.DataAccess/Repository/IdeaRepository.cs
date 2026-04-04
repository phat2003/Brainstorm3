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
    public class IdeaRepository : Repository<Idea>, IIdeaRepository
    {
        private readonly ApplicationDbContext _db;//biến này chỉ được đọc(không được ghi hay làm gì khác).

        public IdeaRepository(ApplicationDbContext db) : base(db) //gọi đến constructor của lớp cha Repository<T> để khởi tạo DbSet.
        {
            _db = db;// gán giá trị cho biến _db.
            //_db.Products.Include(u => u.Category).Include(u=>u.CoverType);
        }
        public void Save()
        {
            _db.SaveChanges(); //lưu các thay đổi vào cơ sở dữ liệu.
        }
        public void Update(Idea idea)
        {
            var objFromDb = _db.Ideas.FirstOrDefault(u => u.Id == idea.Id);//tìm ý tưởng trong cơ sở dữ liệu dựa trên Id.
            if (objFromDb != null)
            {
                objFromDb.Text = idea.Text;
                objFromDb.FilePath = idea.FilePath;
                objFromDb.CreatedDate = idea.CreatedDate;
                objFromDb.CategoryId = idea.CategoryId;
                objFromDb.TopicId = idea.TopicId;
                objFromDb.ApplicationUserId = idea.ApplicationUserId;
                if (idea.FilePath != null)
                {
                    objFromDb.FilePath = idea.FilePath;
                }
                
                // Cập nhật các thuộc tính khác của ý tưởng nếu cần thiết
            }
            
        }
    }
}
