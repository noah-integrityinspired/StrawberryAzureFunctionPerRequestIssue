using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScopedIssue;
using StrawberryShake;

namespace Company.Function
{
    public class ScopedIssue
    {
        private readonly WorkaroundContext _workaroundContext;
        private readonly ConferenceClient _conferenceClient;

        public ScopedIssue(WorkaroundContext workaroundContext, ConferenceClient conferenceClient)
        {
            _workaroundContext = workaroundContext;
            _conferenceClient = conferenceClient;
        }


        [FunctionName("ScopedIssue")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            _workaroundContext.Secret = "foobar"; // For sake of simplicity hard coded, but imagine I'm pulling this off req
            var result = await _conferenceClient.GetSessions.ExecuteAsync();
            result.EnsureNoErrors();
            return new OkObjectResult(result.Data.Sessions.Nodes);
        }
    }
}
