using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;

namespace HRISAPI.Infrastructure.Repositories
{
    public class WorkflowActionRepository : Repository<WorkflowAction>, IWorkflowActionRepository
    {
        private readonly MyDbContext _db;
        public WorkflowActionRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
