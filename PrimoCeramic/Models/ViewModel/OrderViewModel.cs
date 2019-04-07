using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Models.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public string PersonName { get; set; }

        public string PersonEmail { get; set; }
        public int ProductId { get; set; }

        public int ProductsSelectedForOrderid { get; set; }
        public string ProductName { get; set; }

        public string ProductImge { get; set; }

        public double Price { get; set; }

        public double Quantity { get; set; }

        public DateTime Date { get; set; }

        public string Country { get; set; }



        public string City { get; set; }


        public string Address { get; set; }


        public string Phone { get; set; }

        public bool IsSent { get; set; }

        public bool IsPaid { get; set; }

        public string ProductType { get; set; }

        public string OrderStatus { get; set; }

        public double Vat { get; set; }
    }

}
