using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Database.Context.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public required string CustomerName { get; set; }

        [Precision(18, 2)]
        public decimal OrderValue { get; set; }

        public DateOnly OrderDate { get; set; }
    }
}
