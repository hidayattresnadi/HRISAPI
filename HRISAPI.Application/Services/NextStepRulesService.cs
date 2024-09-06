using HRISAPI.Application.DTO.NextStepRules;
using HRISAPI.Application.IServices;
using HRISAPI.Domain.IRepositories;
using HRISAPI.Domain.Models;

namespace HRISAPI.Application.Services
{
    public class NextStepRulesService : INextStepRulesService
    {
        private readonly INextStepRulesRepository _nextStepRulesRepository;
        public NextStepRulesService(INextStepRulesRepository nextStepRulesRepository )
        {
            _nextStepRulesRepository = nextStepRulesRepository;
        }
        public async Task<bool> AddNextStepRules(NextStepRulesAdd request)
        {
            var newNextStepRules = new NextStepRules
            {
                ConditionType = request.ConditionType,
                ConditionValue = request.ConditionValue,
                CurrentStepId = request.CurrentStepId,
                NextStepId = request.NextStepId
            };
            await _nextStepRulesRepository.AddAsync(newNextStepRules);
            await _nextStepRulesRepository.SaveAsync();
            return true;
        }
    }
}
