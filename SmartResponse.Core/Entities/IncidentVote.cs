namespace SmartResponse.Core.Entities
{
    public class IncidentVote : BaseEntity
    {
        public Guid IncidentId { get; set; }
        public virtual Incident Incident { get; set; } = null!;
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public int VoteValue { get; set; } // +1 for Upvote, -1 for Downvote
    }
}
