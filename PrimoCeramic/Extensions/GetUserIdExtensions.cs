using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrimoCeramic.Extensions
{
    public static class GetUserIdExtensions
    {

        public static string GetUserIdex(this ClaimsPrincipal user)
        {

            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }

                

            ClaimsPrincipal currentUser = user;

            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            
        }
    }
}
