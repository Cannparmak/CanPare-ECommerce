using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class OrderItem : IEntity
    {
        public int Id { get; set; }
        
        [Display(Name = "Sipariş")]
        public int OrderId { get; set; }
        
        [Display(Name = "Sipariş")]
        public Order? Order { get; set; }
        
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }
        
        [Display(Name = "Ürün")]
        public Product? Product { get; set; }
        
        [Display(Name = "Miktar")]
        public int Quantity { get; set; }
        
        [Display(Name = "Birim Fiyat")]
        public decimal UnitPrice { get; set; }
        
        [Display(Name = "Toplam Tutar")]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
} 