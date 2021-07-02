using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MacDonalsApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public byte[] ImageArray { get; set; }
        public double Price { get; set; }
        public bool IsPopularProduct { get; set; }

        public ICollection<OrderDetail> orderDetails { get; set; }
        public ICollection<ShoppingCartItem> shoppingCartItems { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
