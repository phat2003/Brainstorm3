using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }//thuộc tính Category để truy cập các phương thức của CategoryRepository.
        ITopicRepository Topic { get; }//thuộc tính Topic để truy cập các phương thức của TopicRepository.
        IIdeaRepository Idea { get; }//thuộc tính Idea để truy cập các phương thức của IdeaRepository.
        IDepartmentRepository Department { get; }//thuộc tính Department để truy cập các phương thức của DepartmentRepository.
        IApplicationUserRepository ApplicationUser { get; }//thuộc tính ApplicationUser để truy cập các phương thức của ApplicationUserRepository.
        IViewRepository View { get; }//thuộc tính View để truy cập các phương thức của ViewRepository.
        IReactRepository React { get; }//thuộc tính React để truy cập các phương thức của ReactRepository.
        ICommentRepository Comment { get; }//thuộc tính Comment để truy cập các phương thức của CommentRepository.
        void Save(); //phương thức Save để lưu các thay đổi vào cơ sở dữ liệu.
    }
}
