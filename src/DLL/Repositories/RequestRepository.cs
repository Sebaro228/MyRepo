using DLL.Context;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        RequestDbContext _dbContext;
        public RequestRepository(RequestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Request request)
        {

            await _dbContext.RequestHistory.AddAsync(request);
            await _dbContext.SaveChangesAsync();
        }
    }
}
