using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace EventNotifier.Infrastructure.Filters
{

    //TODO : fix it
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly TokenValidatedContext _tokenValidatedContext;
        public HangfireAuthorizationFilter(TokenValidatedContext tokenValidatedContext)
        {
            _tokenValidatedContext = tokenValidatedContext;
        }
        public HangfireAuthorizationFilter() { }
        public bool Authorize([NotNull] DashboardContext context)
        {
            var isAuth = _tokenValidatedContext.Result.Succeeded;
            return isAuth;
        }

    }
}
