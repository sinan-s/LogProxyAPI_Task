using LogProxyAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogProxyAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class ProxyController : ControllerBase
    {
        static string API_KEY = "key46INqjpp7lMzjd";
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(ILogger<ProxyController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<LogIncoming>> Get()
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_KEY);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await httpClient.GetAsync("https://api.airtable.com/v0/appD1b1YjWoXkUJwR/Messages?maxRecords=3&view=Grid%20view"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var messages = JsonSerializer.Deserialize<LogOutgoingRoot>(apiResponse);
                        return messages.records.Select(s=> new LogIncoming { id = s.fields.id, receivedAt = s.fields.receivedAt, text = s.fields.Message, title = s.fields.Summary });
                    }
                    else
                    {

                        throw new Exception("Error: " + apiResponse);
                    }
                }
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LogIncoming log)
        {
            var rng = new Random();

            LogOutgoing outgoing = new LogOutgoing
            {
                id = rng.Next(1, 1000).ToString(),
                receivedAt = DateTime.Now,
                Summary = log.title,
                Message = log.text
            };
            LogOutgoingRecord logOutgoingRecord = new LogOutgoingRecord { fields = outgoing };
            LogOutgoingRoot logOutgoingRoot = new LogOutgoingRoot { records = new List<LogOutgoingRecord> { logOutgoingRecord } };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_KEY);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new StringContent(JsonSerializer.Serialize(logOutgoingRoot), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://api.airtable.com/v0/appD1b1YjWoXkUJwR/Messages", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogInformation("incoming log data successfully transferred to third party log api");

                        return Ok();
                    }
                    else
                    {
                        _logger.LogInformation($"an error has occured while transferring to third party log api: {apiResponse}");

                        return StatusCode(response.StatusCode.GetHashCode(), apiResponse);
                    }
                }
            }

        }

    }
}
