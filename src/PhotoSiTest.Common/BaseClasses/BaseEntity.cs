using PhotoSiTest.Common.BaseInterfaces;
using System.ComponentModel.DataAnnotations;

namespace PhotoSiTest.Common.BaseClasses
{
    public abstract class BaseEntity : ISoftDeletable
    {
        [Required]
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
