using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;

namespace HRISAPI.Infrastructure.Repositories
{
    public class WorkflowRepository : Repository<Workflow>, IWorkflowRepository
    {
        private readonly MyDbContext _db;
        public WorkflowRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
