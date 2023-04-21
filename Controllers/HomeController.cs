using System.Diagnostics;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Catpointer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catpointer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EventHubProducerClient _producer;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            var eventHubsConnectionString = configuration["connectionString"];
            var eventHubName = configuration["eventHubName"];

            _producer = new EventHubProducerClient(eventHubsConnectionString, eventHubName);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Activate()
        {


            try
            {
                using EventDataBatch eventBatch = await _producer.CreateBatchAsync();

                for (var counter = 0; counter < int.MaxValue; ++counter)
                {
                    var eventBody = new BinaryData($"Activate");
                    var eventData = new EventData(eventBody);

                    if (!eventBatch.TryAdd(eventData))
                    {
                        // At this point, the batch is full but our last event was not
                        // accepted.  For our purposes, the event is unimportant so we
                        // will intentionally ignore it.  In a real-world scenario, a
                        // decision would have to be made as to whether the event should
                        // be dropped or published on its own.

                        break;
                    }
                }

                // When the producer publishes the event, it will receive an
                // acknowledgment from the Event Hubs service; so long as there is no
                // exception thrown by this call, the service assumes responsibility for
                // delivery.  Your event data will be published to one of the Event Hub
                // partitions, though there may be a (very) slight delay until it is
                // available to be consumed.

                await _producer.SendAsync(eventBatch);
            }
            catch
            {
                // Transient failures will be automatically retried as part of the
                // operation. If this block is invoked, then the exception was either
                // fatal or all retries were exhausted without a successful publish.
            }
            finally
            {
                await _producer.CloseAsync();
            }
            return View(viewName: "Activated");
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate()
        {
            try
            {
                using EventDataBatch eventBatch = await _producer.CreateBatchAsync();

                for (var counter = 0; counter < int.MaxValue; ++counter)
                {
                    var eventBody = new BinaryData($"Deactivate");
                    var eventData = new EventData(eventBody);

                    if (!eventBatch.TryAdd(eventData))
                    {
                        // At this point, the batch is full but our last event was not
                        // accepted.  For our purposes, the event is unimportant so we
                        // will intentionally ignore it.  In a real-world scenario, a
                        // decision would have to be made as to whether the event should
                        // be dropped or published on its own.

                        break;
                    }
                }

                // When the producer publishes the event, it will receive an
                // acknowledgment from the Event Hubs service; so long as there is no
                // exception thrown by this call, the service assumes responsibility for
                // delivery.  Your event data will be published to one of the Event Hub
                // partitions, though there may be a (very) slight delay until it is
                // available to be consumed.

                await _producer.SendAsync(eventBatch);
            }
            catch
            {
                // Transient failures will be automatically retried as part of the
                // operation. If this block is invoked, then the exception was either
                // fatal or all retries were exhausted without a successful publish.
            }
            finally
            {
                await _producer.CloseAsync();
            }
            return View(viewName: "Index");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}