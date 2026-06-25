namespace SampleAPI.Database.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public required string CustomerName { get; set; }
        public decimal OrderValue { get; set; }
        public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
