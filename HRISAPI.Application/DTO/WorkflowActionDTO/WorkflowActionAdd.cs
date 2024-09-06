namespace HRISAPI.Application.DTO.WorkflowActionDTO
{
    public class WorkflowActionAdd
    {
        public int ProcessId { get; set; }
        public int StepId { get; set; }
        public string ActorId { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        public string Comments { get; set; }
    }
}
