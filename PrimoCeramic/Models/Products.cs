using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class Products
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public bool Available { get; set; }

        public string ShadeColor { get; set; }

        public string Image { get; set; }

        public string ImageFolder { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Depth { get; set; }

        public double Casepackaging { get; set; }
        
        public double Thickness { get; set; }

        public double Weight { get; set; }

        public string Note { get; set; }

        public bool Deleted { get; set; }

        [Display(Name="Product Type")]
        public int ProductTypeId { get; set; }

        [ForeignKey("ProductTypeId")]
        public virtual ProductTypes ProductTypes { get; set; }

        [Display(Name = "Special Tag ")]
        public int SpecialTagsId { get; set; }

        [ForeignKey("SpecialTagsId")]
        public virtual SpecialTags SpecialTags { get; set; }

        [Display(Name = "Product Country")]
        public int ProductCountryId { get; set; }

        [ForeignKey("ProductCountryId")]
        public virtual ProductCountry ProductCountry { get; set; }

        [Display(Name = "Product Surface")]
        public int ProductSurfaceId { get; set; }

        [ForeignKey("ProductSurfaceId")]
        public virtual ProductSurface ProducSurface { get; set; }

        [Display(Name = "Product Application")]
        public int ProductApplicationId { get; set; }

        [ForeignKey("ProductApplicationId")]
        public virtual ProductApplication ProductApplication { get; set; }
    }
}
