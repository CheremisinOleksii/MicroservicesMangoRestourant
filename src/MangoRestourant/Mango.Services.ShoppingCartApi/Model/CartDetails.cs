using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartApi.Model
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }
        
        public int CartHeaderId { get; set; }
       
        [ForeignKey("CartHeaderId")]
        public virtual CartHeader CartHeader { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Count { get; set; }
    }
}
