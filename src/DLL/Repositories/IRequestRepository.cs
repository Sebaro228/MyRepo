using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DLL.Repositories
{
    public interface IRequestRepository
    {
        Task AddAsync(Request request);
    }
}