using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Slider : IEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Resim alanı zorunludur.")]
        [Display(Name = "Resim")]
        public string Image { get; set; }
        public string? Link { get; set; }
        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }
        [Display(Name = "Kategori")]
        public Category? Category { get; set; }

    }
}
