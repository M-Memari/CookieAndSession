using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace CookieAndSession
{
    public class ConsentMiddleware
    {
        private readonly RequestDelegate _next;

        public ConsentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/consent")
            {
                var consentFeature = context.Features.Get<ITrackingConsentFeature>();
                if (!consentFeature.HasConsent)
                {
                    consentFeature.GrantConsent();
                }
                else
                {
                    consentFeature.WithdrawConsent();
                }

                await context.Response.WriteAsync(consentFeature.HasConsent
                    ? "Consent Granted\n"
                    : "Consent Withdrawn\n");
            }
            else
            {
                await _next(context);
            }
        }
    }
}
