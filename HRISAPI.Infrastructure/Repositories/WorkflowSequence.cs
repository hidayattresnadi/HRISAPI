using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;

namespace HRISAPI.Infrastructure.Repositories
{
    public class WorkflowSequenceRepository : Repository<WorkflowSequence>, IWorkflowSequenceRepository
    {
        private readonly MyDbContext _db;
        public WorkflowSequenceRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
