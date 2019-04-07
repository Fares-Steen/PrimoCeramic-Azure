using PrimoCeramic.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.HTMLTemplate
{
    public class OrderHTML
    {

        public List<OrderViewModel> OrderViewModel { get; set; }
       
        public OrderHTML(List<OrderViewModel> OrderViewModel)
        {
            this.OrderViewModel = OrderViewModel;
            
        }
        
                

        public string HtmalGenerated()
        {
           

            string Html = "<!DOCTYPEhtml><html><head><style>table{font-family:arial,sans-serif;border-collapse:collapse;width:100%;}td,th{border:1pxsolid#dddddd;text-align:left;padding:8px;}tr:nth-child(even){background-color:#dddddd;}</style></head><body><h5>Thank you for your order.....</h5><br /><h5>We will contact you soon for delivery and payment.</h5><br /><h3>Your Order Details:</h3>";
            Html +=String.Format("<div><table><tr style='background-color:#1f3933;color:white'><th style='text-align:center;width:20%'>{0}</th><th style='text-align:center;'><label>ProductName</label></th><th style='text-align:center;'><label>Type</label></th><th style='text-align:center;'><label>QTY</label></th><th style='text-align:center;'><label>Price</label></th><th style='text-align:center;'><label>Total</label></th><th style='text-align:center;'><label>VAT</label></th><th style='text-align:center;'><label>Total with VAT</label></th></tr>", OrderViewModel.FirstOrDefault().OrderId);
            double Sum = 0;
            double SumVat = 0;
            double SumWithVat = 0;
            foreach (var item in OrderViewModel)
            {
                Sum += (item.Quantity * item.Price);
                double total = item.Price * item.Quantity;
                double vat = total * item.Vat / 100;
                double itemtotalvat = vat + total;
                SumVat += vat;
                SumWithVat += itemtotalvat;
                Html += String.Format("<tr style='padding:50px'><td><img src='{0}' width='50px' style='margin:auto; display:block;border-radius:5px;border:1px solid #bbb9b9' /></td><td style='text-align:center'>{1}</td><td style='text-align:center'>{2}</td><td style='text-align:center'>{3}</td><td style='text-align:center'>{4}</td><td style='text-align:center'>{5}</td><td style='text-align:center'>{6}</td><td style='text-align:center'>{7}</td></tr>", "https://primoceramic.azurewebsites.net" + item.ProductImge, item.ProductName, item.ProductType, item.Quantity, item.Price, total,vat,itemtotalvat);
            }
            Html += String.Format("<tr style='background-color:#1f3933;color:white'><th style='text-align:center; width:30%;'>{0}</th><th style='text-align:center;'><label></label></th><th style='text-align:center;'><label></label></th><th style='text-align:center;'></th><th style='text-align:center;'><label>Total</label></th><th style='text-align:center;'><label>{1} S.R</label></th><th style='text-align:center;'><label>{2} S.R</label></th><th style='text-align:center;'><label>{3} S.R</label></th></tr></table></div>", OrderViewModel.FirstOrDefault().Date.ToShortDateString(),Sum,SumVat,SumWithVat);
            Html += "<br/><br/><br/><div><a href='https://www.primoceramic.com' style='text-decoration:none;color: #1f3933'>PrimoCermaic</a><br/><br/><a href='mailto:info@primoceramic.com' style='text-decoration:none;color: #1f3933'>info@primoceramic.com</a></div></body></html>";
            return Html;
        }
    }
}
