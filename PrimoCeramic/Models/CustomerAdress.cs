using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class CustomerAddress
    {
        public int Id { get; set; }

        [Display(Name = "Person")]
        public string PersonID { get; set; }

        [ForeignKey("PersonID")]
        public virtual ApplicationUser Person { get; set; }

        [Required]
        public string Country { get; set; }


        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Phone { get; set; }

        public bool IsDefault { get; set; }
    }
}
