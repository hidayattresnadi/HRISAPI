using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;
using HRISAPI.Infrastructure.Context;

namespace HRISAPI.Infrastructure.Repositories
{
    public class NextStepRulesRepository : Repository<NextStepRules>, INextStepRulesRepository
    {
        private readonly MyDbContext _db;
        public NextStepRulesRepository(MyDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
