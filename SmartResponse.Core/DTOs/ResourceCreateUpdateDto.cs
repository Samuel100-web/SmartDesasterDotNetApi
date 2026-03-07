namespace SmartResponse.Core.DTOs
{
    // Resource create/update ke liye
    public record ResourceCreateUpdateDto(
        string Name,
        Guid CategoryId,
        int TotalQty,
        int AvailableQty,
        decimal Lat,
        decimal Long
    );
}
