using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrimoCeramic.Utility
{
    public class GetLogedId
    {
        public string UserId { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetLogedId(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetLogedIdValue()
        {
            UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return UserId;
        }
        public string GetLogedIdNoValue()
        {

            UserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return UserId;
        }

    }
}
