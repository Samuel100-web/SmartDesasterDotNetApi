namespace SmartResponse.Core.DTOs
{
    public record VoteCreateDto(Guid IncidentId, int VoteValue); // +1 or -1
}
