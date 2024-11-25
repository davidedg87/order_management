using System.ComponentModel.DataAnnotations;

namespace PhotoSi.UserService.Core.Dtos
{
    public class UserEditDto
    {
        public int? Id { get; set; }              // Id univoco utente
        [Required]
        public string Name { get; set; }        // Nome
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }       // Email
    }
}
