using System.ComponentModel.DataAnnotations;

namespace Eticaret.WebUI.Models
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [Display(Name = "İsim")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyisim alanı zorunludur.")]
        [Display(Name = "Soyisim")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; }
    }
}
