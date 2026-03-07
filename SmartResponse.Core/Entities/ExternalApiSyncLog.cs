namespace SmartResponse.Core.Entities
{
    // 10. External API Sync Logs
    public class ExternalApiSyncLog : BaseEntity
    {
        public string ProviderName { get; set; } = string.Empty; // USGS, GDACS
        public DateTime SyncTime { get; set; }
        public string Status { get; set; } = "Success";
        public int RecordsAdded { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
