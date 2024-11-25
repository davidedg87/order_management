using PhotoSiTest.OrderService.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace PhotoSi.OrderService.Core.Dtos
{
    public class OrderEditDto    
    {
        public int? Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        public List<int> ProductIds { get; set; }
        [Required]
        public int AddressId { get; set; }
        [Required]
        public OrderStatus? Status { get; set; } // Stato dell'ordine
    }
}
