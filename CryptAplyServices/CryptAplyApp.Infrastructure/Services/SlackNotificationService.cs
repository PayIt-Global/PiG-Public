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
    public class SlackNotificationService : ISlackNotificationService
    {
        private readonly ILogger<SlackNotificationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly SlackOptions _options;

        public SlackNotificationService(
            ILogger<SlackNotificationService> logger,
            IOptions<SlackOptions> options,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _options = options.Value;
            _httpClient = httpClientFactory.CreateClient("slack");
            _httpClient.BaseAddress = new Uri("https://slack.com/api/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.BotToken}");
        }

        public async Task SendMessageAsync(string channel, SlackMessage message)
        {
            try
            {
                var payload = new
                {
                    channel,
                    text = message.Text,
                    blocks = message.Blocks,
                };

                var response = await PostToSlackAsync("chat.postMessage", payload);
                if (!response.ok)
                {
                    throw new Exception($"Failed to send Slack message: {response.error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Slack message to channel {Channel}", channel);
                throw;
            }
        }

        public async Task UpdateMessageAsync(string messageTs, SlackMessage message)
        {
            try
            {
                var payload = new
                {
                    ts = messageTs,
                    text = message.Text,
                    blocks = message.Blocks,
                };

                var response = await PostToSlackAsync("chat.update", payload);
                if (!response.ok)
                {
                    throw new Exception($"Failed to update Slack message: {response.error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Slack message {MessageTs}", messageTs);
                throw;
            }
        }

        private async Task<(bool ok, string error)> PostToSlackAsync<T>(string endpoint, T payload)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            
            return (
                root.GetProperty("ok").GetBoolean(),
                root.TryGetProperty("error", out var error) ? error.GetString() : null
            );
        }
    }

    public class SlackOptions
    {
        public string BotToken { get; set; }
    }
}
