namespace SmartResponse.Core.DTOs
{
    public record SyncLogDto(
    string ProviderName,
    DateTime SyncTime,
    string Status,
    int RecordsAdded,
    string? ErrorMessage
);
}
