using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;

namespace HRISAPI.Infrastructure.Repositories
{
    public class ProcessRepository : Repository<Process>, IProcessRepository
    {
        private readonly MyDbContext _db;
        public ProcessRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
