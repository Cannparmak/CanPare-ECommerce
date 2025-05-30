﻿using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class News : IEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Duyuru Adı alanı zorunludur.")]
        [Display(Name = "Adı")]
        public string Name { get; set; }
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        [Display(Name = "Resim")]
        public string? Image { get; set; }
        [Display(Name = "Aktif mi ?")]
        public bool IsActive { get; set; }
        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }
        [Display(Name = "Kategori")]
        public Category? Category { get; set; }
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
