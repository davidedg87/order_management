using Microsoft.EntityFrameworkCore;
using PhotoSiTest.Common.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace PhotoSiTest.UserService.Core.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
