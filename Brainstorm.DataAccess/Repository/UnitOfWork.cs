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
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }// thuộc tính chỉ có thể được nhận(get) và gán(set) giá trị bên trong lớp này.
        public ITopicRepository Topic { get; private set; }
        public IIdeaRepository Idea { get; private set; }
        public IDepartmentRepository Department { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IViewRepository View { get; private set; }
        public IReactRepository React { get; private set; }
        public ICommentRepository Comment { get; private set; }

        private readonly ApplicationDbContext _db;//biến này chỉ được đọc(không được ghi hay làm gì khác).

        public UnitOfWork(ApplicationDbContext db) //gọi đến constructor của lớp cha Repository<T> để khởi tạo DbSet.
        {
            _db = db;// gán giá trị cho biến _db.
            Category = new CategoryRepository(_db);//khởi tạo CategoryRepository và gán nó cho thuộc tính Category.
            Topic = new TopicRepository(_db);
            Idea = new IdeaRepository(_db);
            Department = new DepartmentRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            View = new ViewRepository(_db);
            React = new ReactRepository(_db);
            Comment = new CommentRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges(); //lưu các thay đổi vào cơ sở dữ liệu.
        }
    }
}
