using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CryptAplyApp.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CryptAplyApp.Infrastructure.Services
{
    public class PagerDutyService : IPagerDutyService
    {
        private readonly ILogger<PagerDutyService> _logger;
        private readonly HttpClient _httpClient;
        private readonly PagerDutyOptions _options;

        public PagerDutyService(
            ILogger<PagerDutyService> logger,
            IOptions<PagerDutyOptions> options,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _options = options.Value;
            _httpClient = httpClientFactory.CreateClient("pagerduty");
            _httpClient.BaseAddress = new Uri("https://api.pagerduty.com/");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.pagerduty+json;version=2");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token token={_options.ApiToken}");
        }

        public async Task CreateIncidentAsync(PagerDutyIncident incident)
        {
            try
            {
                var payload = new
                {
                    incident = new
                    {
                        type = "incident",
                        title = incident.Title,
                        service = new
                        {
                            id = _options.ServiceId,
                            type = "service_reference"
                        },
                        urgency = MapSeverityToUrgency(incident.Severity),
                        body = new
                        {
                            type = "incident_body",
                            details = incident.Description
                        },
                        custom_details = incident.CustomDetails
                    }
                };

                var response = await PostToPagerDutyAsync("incidents", payload);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create PagerDuty incident: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PagerDuty incident for {Title}", incident.Title);
                throw;
            }
        }

        public async Task AcknowledgeIncidentAsync(string incidentId)
        {
            try
            {
                var payload = new
                {
                    incident = new
                    {
                        type = "incident_reference",
                        status = "acknowledged"
                    }
                };

                var response = await PutToPagerDutyAsync($"incidents/{incidentId}", payload);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to acknowledge PagerDuty incident: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acknowledging PagerDuty incident {IncidentId}", incidentId);
                throw;
            }
        }

        public async Task ResolveIncidentAsync(string incidentId)
        {
            try
            {
                var payload = new
                {
                    incident = new
                    {
                        type = "incident_reference",
                        status = "resolved"
                    }
                };

                var response = await PutToPagerDutyAsync($"incidents/{incidentId}", payload);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to resolve PagerDuty incident: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving PagerDuty incident {IncidentId}", incidentId);
                throw;
            }
        }

        private async Task<HttpResponseMessage> PostToPagerDutyAsync<T>(string endpoint, T payload)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            return await _httpClient.PostAsync(endpoint, content);
        }

        private async Task<HttpResponseMessage> PutToPagerDutyAsync<T>(string endpoint, T payload)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            return await _httpClient.PutAsync(endpoint, content);
        }

        private static string MapSeverityToUrgency(string severity)
        {
            return severity.ToLower() switch
            {
                "critical" => "high",
                "high" => "high",
                _ => "low"
            };
        }
    }

    public class PagerDutyOptions
    {
        public string ApiToken { get; set; }
        public string ServiceId { get; set; }
    }
}
