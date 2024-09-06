namespace HRISAPI.Application.DTO.ProcessDTO
{
    public class ProcessAdd
    {
        public int WorkflowId { get; set; }
        public string RequesterId { get; set; }
        public string RequestType { get; set; }
        public string Status { get; set; }
        public int CurrentStepId { get; set; }
        public int RequestId { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
