using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models
{
    public class SpecialTags
    {
        public int Id { get; set; }

      

        [Required]
        public string Name { get; set; }

    }
}
