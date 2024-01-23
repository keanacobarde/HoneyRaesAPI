namespace HoneyRaesAPI.Models
{
    public class ServiceTicket
    {
        int Id { get; set; }
        int CustomerId { get; set; }
        int EmployeeId { get; set; }
        public string Description { get; set; }
        bool Emergency { get; set; }
        DateTime DateCompleted { get; set; }
    }
}
