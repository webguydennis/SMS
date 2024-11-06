using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run the task every 10 minutes for example
        var interval = TimeSpan.FromMinutes(10);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Call the API
                await HitApiEndpointAsync();
                
                // Set the time for the task to run every day at 2:00 AM
                var now = DateTime.Now;
                var nextRunTime = DateTime.Today.AddDays(1).AddHours(2); // 2:00 AM tomorrow

                var delayTime = nextRunTime - now;
                if (delayTime.TotalMilliseconds <= 0)
                {
                    delayTime = TimeSpan.FromMilliseconds(1); // To avoid negative delay
                }

                // Log the next scheduled task time
                _logger.LogInformation($"Next task will run at {nextRunTime:HH:mm:ss}");

                // Wait until the calculated next run time
                await Task.Delay(delayTime, stoppingToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while hitting the API.");
            }
        }
    }

    private async Task HitApiEndpointAsync()
    {
        var client = _httpClientFactory.CreateClient();

        // Use your actual API URL here
        var apiUrl = "http://localhost:5000/sms/RemoveInActiveSms";

        // Send a Delete request to the API
        var response = await client.DeleteAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("API Response: {response}", content);
        }
        else
        {
            _logger.LogWarning("API call failed with status code: {statusCode}", response.StatusCode);
        }
    }
}
