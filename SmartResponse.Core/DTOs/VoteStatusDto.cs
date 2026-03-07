using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResponse.Core.DTOs
{
    public record VoteStatusDto(int TotalVotes, int? UserVoteValue);
}
