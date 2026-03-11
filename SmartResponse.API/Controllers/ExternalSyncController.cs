using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;

namespace SmartResponse.API.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalSyncController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExternalSyncController> _logger;

        public ExternalSyncController(IUnitOfWork uow, IHttpClientFactory httpClientFactory, ILogger<ExternalSyncController> logger)
        {
            _uow = uow;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // 1. GET: Sync History (Matches SyncLogDto)
        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<SyncLogDto>>> GetSyncLogs()
        {
            var logs = await _uow.SyncLogs.GetAllAsync();
            var result = logs.OrderByDescending(l => l.SyncTime).Select(l => new SyncLogDto(
                l.ProviderName, l.SyncTime, l.Status, l.RecordsAdded, l.ErrorMessage
            ));
            return Ok(result);
        }

        // 2. POST: Manual Sync Trigger (Example: USGS Earthquakes)
        [HttpPost("sync-usgs")]
        public async Task<IActionResult> SyncUSGS()
        {
            var log = new ExternalApiSyncLog { ProviderName = "USGS", SyncTime = DateTime.UtcNow };
            try
            {
                var client = _httpClientFactory.CreateClient();
                // USGS API hit karna
                var response = await client.GetAsync("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&minmagnitude=5");

                if (!response.IsSuccessStatusCode) throw new Exception("USGS API unreachable");

                var data = await response.Content.ReadFromJsonAsync<dynamic>();
                int addedCount = 0;                

                log.Status = "Success";
                log.RecordsAdded = addedCount;
            }
            catch (Exception ex)
            {
                log.Status = "Failed";
                log.ErrorMessage = ex.Message;
                _logger.LogError(ex, "USGS Sync Failed");
            }

            await _uow.SyncLogs.AddAsync(log);
            await _uow.CompleteAsync();

            return Ok(log);
        }
    }
}
