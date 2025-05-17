using System.ComponentModel.DataAnnotations;
using Eticaret.Core.Entities;
using Eticaret.WebUI.Models;

namespace Eticaret.WebUI.Models
{
    public class CheckoutViewModel
    {
        public List<CartLine> CartLines { get; set; } = new List<CartLine>();
        public decimal TotalPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal GrandTotal => TotalPrice + ShippingCost;

        // Teslimat Bilgileri
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        public string FirstName { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        public string LastName { get; set; }

        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; }

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "Telefon alanı zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Phone { get; set; }

        [Display(Name = "Adres")]
        [Required(ErrorMessage = "Adres alanı zorunludur")]
        public string Address { get; set; }

        [Display(Name = "İl")]
        [Required(ErrorMessage = "İl alanı zorunludur")]
        public string City { get; set; }

        [Display(Name = "İlçe")]
        [Required(ErrorMessage = "İlçe alanı zorunludur")]
        public string District { get; set; }

        [Display(Name = "Posta Kodu")]
        [Required(ErrorMessage = "Posta kodu alanı zorunludur")]
        public string ZipCode { get; set; }

        // Ödeme Bilgileri
        [Display(Name = "Kart Üzerindeki İsim")]
        [Required(ErrorMessage = "Kart üzerindeki isim alanı zorunludur")]
        public string CardholderName { get; set; }

        [Display(Name = "Kart Numarası")]
        [Required(ErrorMessage = "Kart numarası alanı zorunludur")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Kart numarası 16 haneli olmalıdır")]
        public string CardNumber { get; set; }

        [Display(Name = "Son Kullanma Tarihi (AA/YY)")]
        [Required(ErrorMessage = "Son kullanma tarihi alanı zorunludur")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Son kullanma tarihi AA/YY formatında olmalıdır")]
        public string ExpiryDate { get; set; }

        [Display(Name = "CVV")]
        [Required(ErrorMessage = "CVV alanı zorunludur")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV 3 haneli olmalıdır")]
        public string CVV { get; set; }
    }
} 