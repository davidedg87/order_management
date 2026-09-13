using OrderManagement.Common.BaseInterfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Common.BaseClasses
{
    public abstract class BaseEntity : ISoftDeletable
    {
        [Required]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
