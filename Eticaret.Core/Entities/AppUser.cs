using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class AppUser : IEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [Display(Name = "İsim")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyisim alanı zorunludur.")]
        [Display(Name = "Soyisim")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Display(Name = "Adres")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [Display(Name = "Şifre")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; }
        [Display(Name = "Aktif mi ?")]
        public bool IsActive { get; set; }
        [Display(Name = "Admin mi ?")]
        public bool IsAdmin { get; set; }
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)] 
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [ScaffoldColumn(false)]
        public Guid? UserGuid { get; set; } = Guid.NewGuid();
        public IList<Comment> Comments { get; set; } = new List<Comment>();
        public IList<Order> Orders { get; set; } = new List<Order>();
        public IList<Cart> Carts { get; set; } = new List<Cart>();
        public IList<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
