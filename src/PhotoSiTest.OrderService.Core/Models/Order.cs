using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace PhotoSiTest.OrderService.Core.Models
{
    // Enum per rappresentare i possibili stati dell'ordine
    public enum OrderStatus
    {
        Pending,        // In attesa
        Processing,     // In elaborazione
        Completed,      // Completato
        Cancelled       // Annullato
    }

    [Index(nameof(UserId), nameof(AddressId), nameof(OrderDate), IsUnique = true)]
    public class Order : BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public List<int> ProductIds { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        [Required]
        public int AddressId { get; set; }
    }
}
