using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities

{
    public class Comment : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Kullanıcı")]
        public AppUser? AppUser { get; set; }
        [Display(Name = "Kullanıcı")]
        public int AppUserId { get; set; }
        [Display(Name = "Ürün")]
        public Product? Product { get; set; }
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Yorum alanı zorunludur.")]
        [Display(Name = "Yorum")]
        public string Message { get; set; }
        [Display(Name = "Yıldız")]
        public int Rating { get; set; }
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        
    }
}
