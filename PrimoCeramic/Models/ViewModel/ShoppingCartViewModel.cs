using PrimoCeramic.TempModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    [Area("Customer")]
    public class ShoppingCartViewModel
    {

        public List<Products> Products { get; set; }

        public List<ShoppingCart> ShoppingCart { get; set; }

        public Statics Vat { get; set; }


    }
}
