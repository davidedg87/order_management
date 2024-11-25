using PhotoSiTest.OrderService.Core.Models;

namespace PhotoSi.OrderService.Core.Dtos
{
    public class OrderDto    
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }  
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<int> ProductIds { get; set; }
        public List<string> ProductCodes { get; set; }  // Aggiunto per i codici dei prodotti
        public int AddressId { get; set; }
        public string AddressFull { get; set; }  // Indirizzo completo
        public OrderStatus Status { get; set; } // Stato dell'ordine
    }
}
