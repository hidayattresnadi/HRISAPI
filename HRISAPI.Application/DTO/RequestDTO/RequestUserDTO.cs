namespace HRISAPI.Application.DTO.RequestDTO
{
    public class RequestUserDTO
    {
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null;
        public int WorkflowId { get; set; }
    }
}
