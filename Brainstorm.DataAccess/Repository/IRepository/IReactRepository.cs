using Brainstorm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Repository.IRepository
{
    public interface IReactRepository : IRepository<React>
    {
        int IncrementCount(React react, int count);
        int DecrementCount(React react, int count);
        void Update(React obj);
    }
}
