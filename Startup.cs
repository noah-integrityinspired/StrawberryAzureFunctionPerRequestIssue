using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using ScopedIssue;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Company.Function.Startup))]
namespace Company.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            // Also tried AddTransient
            builder.Services.AddScoped<TokenHeaderHandlerGraphQl>();

            builder.Services.AddScoped<WorkaroundContext>();

            builder.Services.AddConferenceClient()
                .ConfigureHttpClient(
                    client => client.BaseAddress = new Uri("https://workshop.chillicream.com/graphql"),
                    builder => builder.AddHttpMessageHandler<TokenHeaderHandlerGraphQl>()
                );

        }
    }

    public class WorkaroundContext
    {
        public string Secret { get; set; }
    }


    public class TokenHeaderHandlerGraphQl : DelegatingHandler
    {
        private readonly WorkaroundContext _workaroundContext;

        public TokenHeaderHandlerGraphQl(WorkaroundContext workaroundContext)
        {
            _workaroundContext = workaroundContext;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Secret should print 'foobar' but will be nothing");
            Console.WriteLine($"Secret: {_workaroundContext.Secret}");
            request.Headers.Add("somesecret", _workaroundContext.Secret);
            return await base.SendAsync(request, cancellationToken);
        }
    }

}