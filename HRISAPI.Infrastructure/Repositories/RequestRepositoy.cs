using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;

namespace HRISAPI.Infrastructure.Repositories
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        private readonly MyDbContext _db;
        public RequestRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
