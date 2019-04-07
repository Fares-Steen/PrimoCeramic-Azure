using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class ProductImages
    {
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }

        public int Id { get; set; }

        public string Image { get; set; }
        public int Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
