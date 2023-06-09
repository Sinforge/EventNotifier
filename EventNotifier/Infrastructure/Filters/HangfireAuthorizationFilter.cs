﻿using EventNotifier.Models;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EventNotifier.Infrastructure.Filters
{

    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {

 
        public bool Authorize([NotNull] DashboardContext context)
        {
            var cookie = context.GetHttpContext().Request.Cookies.FirstOrDefault(p => p.Key == "token");
            if(cookie.Value != null)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken securityToken = handler.ReadToken(cookie.Value) as JwtSecurityToken;
                string userRole = securityToken.Claims.First(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                return userRole == Role.Administration.ToString();
                

            }
            return false;
           
        }

    }
}
