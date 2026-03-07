namespace SmartResponse.Core.DTOs
{
    public record ResourceReadDto(
    Guid Id,
    string Name,
    string CategoryName,
    int AvailableQty,
    int TotalQty,
    decimal Lat,
    decimal Long
);
}
