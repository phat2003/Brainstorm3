using Brainstorm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Repository.IRepository
{
    public interface IViewRepository : IRepository<View>
    {
        int IncrementCount(View view, int count);

    }
}
