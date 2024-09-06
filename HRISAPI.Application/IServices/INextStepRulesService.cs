using HRISAPI.Application.DTO.NextStepRules;

namespace HRISAPI.Application.IServices
{
    public interface INextStepRulesService
    {
        Task<bool> AddNextStepRules(NextStepRulesAdd request);
    }
}
