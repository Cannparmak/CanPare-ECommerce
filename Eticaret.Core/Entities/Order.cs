using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Eticaret.Core.Enums;

namespace Eticaret.Core.Entities
{
    public class Order : IEntity
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
        
        public int Id { get; set; }
        
        [Display(Name = "Sipariş Numarası")]
        public string? OrderNumber { get; set; }
        
        [Display(Name = "Sipariş Tarihi"), ScaffoldColumn(false)]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Kullanıcı")]
        public int? AppUserId { get; set; }
        
        [Display(Name = "Kullanıcı")]
        public AppUser? AppUser { get; set; }

        [Display(Name = "Sipariş Durumu")]
        public OrderStatus Status { get; set; } = OrderStatus.SiparisAlindi;
        
        [Display(Name = "Toplam Tutar")]
        public decimal TotalAmount { get; set; }
        
        // Sipariş detayları
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
