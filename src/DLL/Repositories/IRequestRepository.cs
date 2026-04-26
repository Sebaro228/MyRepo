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
        Task<Request> AddAsync(Request request, string apiKey);
    }
}