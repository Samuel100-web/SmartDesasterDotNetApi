using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Enums;
using SmartResponse.Core.Interfaces;
using System.Net.Http.Json;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;

namespace SmartResponse.Infrastructure.Services
{
    public class ExternalSyncService : IExternalSyncService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<ExternalSyncService> _logger;

        public ExternalSyncService(IUnitOfWork uow, IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<ExternalSyncService> logger)
        {
            _uow = uow;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task SyncAllApis()
        {
            var log = new ExternalApiSyncLog
            {
                ProviderName = "GlobalSync",
                SyncTime = DateTime.UtcNow
            };

            try
            {
                await SyncUSGS();
                await SyncGDACS();
                await SyncWeather();

                log.Status = "Success";
                log.RecordsAdded = 10;
            }
            catch (Exception ex)
            {
                log.Status = "Failed";
                log.ErrorMessage = ex.Message;
            }

            await _uow.SyncLogs.AddAsync(log);
            await _uow.CompleteAsync();
        }

        // 1. USGS Earthquake API (Global Data)
        private async Task SyncUSGS()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                // Magnitude 2.5+ tak ka sara data (Taake boht zyada kachra data bhi na aaye)
                var response = await client.GetAsync("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&minmagnitude=2.5");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var features = data.GetProperty("features");

                    foreach (var feature in features.EnumerateArray())
                    {
                        var props = feature.GetProperty("properties");
                        var coords = feature.GetProperty("geometry").GetProperty("coordinates");

                        decimal lon = coords[0].GetDecimal();
                        decimal lat = coords[1].GetDecimal();
                        string title = props.GetProperty("title").GetString()!;

                        // Filter Hata Diya - Ab sara data save hoga
                        await SaveIncident(
                            title,
                            "Place: " + props.GetProperty("place").GetString(),
                            lat, lon, "USGS",
                            new Guid("67616616-d35d-4f0f-8b54-933e7284897f") // Earthquake ID
                        );
                    }
                }
            }
            catch (Exception ex) { _logger.LogError($"USGS Sync Error: {ex.Message}"); }
        }

        // 2. GDACS Global Alert API (RSS/XML)
        private async Task SyncGDACS()
        {
            try
            {
                using var reader = XmlReader.Create("https://www.gdacs.org/xml/rss.xml");
                var feed = SyndicationFeed.Load(reader);

                foreach (var item in feed.Items)
                {
                    // GDACS se sara data uthana
                    await SaveIncident(
                        item.Title.Text,
                        item.Summary.Text,
                        0, 0, // Note: GDACS XML parsing for Lat/Long requires extra namespaces
                        "GDACS",
                        new Guid("79c72076-905c-426c-829d-6401f8d4e41f") // Flood/General ID
                    );
                }
            }
            catch (Exception ex) { _logger.LogError($"GDACS Sync Error: {ex.Message}"); }
        }

        private async Task SyncWeather()
        {
            try
            {
                string apiKey = _config["OpenWeather:Key"]!;
                var client = _httpClientFactory.CreateClient();

                // Dunia bhar ke 15 Bade Global Hubs (Sampling the Globe)
                var globalCities = new List<(string Name, string Lat, string Lon)>
        {
            ("Islamabad", "33.6844", "73.0479"),
            ("Karachi", "24.8607", "67.0011"),
            ("New York", "40.7128", "-74.0060"),
            ("London", "51.5074", "-0.1278"),
            ("Tokyo", "35.6762", "139.6503"),
            ("Sydney", "-33.8688", "151.2093"),
            ("Dubai", "25.2048", "55.2708"),
            ("Moscow", "55.7558", "37.6173"),
            ("Rio de Janeiro", "-22.9068", "-43.1729"),
            ("Cairo", "30.0444", "31.2357"),
            ("Delhi", "28.6139", "77.2090"),
            ("Mumbai", "19.0760", "72.8777"),
            ("Toronto", "43.6532", "-79.3832"),
            ("Paris", "48.8566", "2.3522"),
            ("Beijing", "39.9042", "116.4074")
        };

                foreach (var city in globalCities)
                {
                    var response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={city.Lat}&lon={city.Lon}&appid={apiKey}&units=metric");

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadFromJsonAsync<JsonElement>();
                        string weatherMain = data.GetProperty("weather")[0].GetProperty("main").GetString()!;
                        string temp = data.GetProperty("main").GetProperty("temp").ToString();

                        
                        await SaveIncident(
                            $"Weather: {city.Name} ({weatherMain})",
                            $"Status: {weatherMain}. Temperature: {temp}°C. Global Update.",
                            decimal.Parse(city.Lat),
                            decimal.Parse(city.Lon),
                            "OpenWeather",
                            new Guid("79c72076-905c-426c-829d-6401f8d4e41f") 
                        );
                    }
                    else
                    {                        
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning($"OpenWeather API Error for {city.Name}: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Weather Sync Failed");
            }
        }

        
        private async Task SaveIncident(string title, string desc, decimal lat, decimal lon, string source, Guid typeId)
        {            
            var existing = await _uow.Incidents.FindAsync(i => i.Title == title);
            if (existing.Any()) return;

            var incident = new Incident
            {
                Title = title,
                Description = desc.Length > 500 ? desc.Substring(0, 497) + "..." : desc,
                TargetLat = lat,
                TargetLong = lon,
                DataSource = source,
                Status = IncidentStatus.Verified,
                Severity = SeverityLevel.Medium,
                IncidentTypeId = typeId,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Incidents.AddAsync(incident);
            await _uow.CompleteAsync();
            _logger.LogInformation($"Saved Incident: {title}");
        }
    }
}