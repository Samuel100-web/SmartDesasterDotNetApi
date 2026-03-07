using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;


namespace SmartResponse.Core.DTOs
{
    public class IncidentMediaUploadDto
    {
        public Guid IncidentId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
