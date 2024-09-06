namespace HRISAPI.Application.DTO.WorkflowSequence
{
    public class WorkflowSequenceAdd
    {
        public int WorkflowId { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; }
        public string? RequiredRole { get; set; }
    }
}
