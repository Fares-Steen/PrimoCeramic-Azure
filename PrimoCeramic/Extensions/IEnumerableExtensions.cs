using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Extensions
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items,int selectedvalue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedvalue.ToString())
                   };
        }
        public static IEnumerable<SelectListItem> ToSelectListItemString<T>(this IEnumerable<T> items, string  selectedvalue)
        {
            if (selectedvalue == null)
            {
                selectedvalue = "";
            }
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedvalue.ToString())
                   };
        }
    }
}
