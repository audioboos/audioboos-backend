using System.ComponentModel.DataAnnotations;

namespace AudioBoos.Data.Models.DTO {
    public class LoginDto {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
    }
}
