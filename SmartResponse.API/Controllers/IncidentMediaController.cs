using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartResponse.Core.DTOs;
using SmartResponse.Core.Entities;
using SmartResponse.Core.Interfaces;
using System;

namespace SmartResponse.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentMediaController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<IncidentMediaController> _logger;

        public IncidentMediaController(IUnitOfWork uow, IWebHostEnvironment env, ILogger<IncidentMediaController> logger)
        {
            _uow = uow;
            _env = env;
            _logger = logger;
        }

        //POST Multiple Images/Videos Upload karna
        [HttpPost("upload")]
        public async Task<IActionResult> UploadMedia([FromForm] IncidentMediaUploadDto dto)
        {
            try
            {
                if (dto.Files == null || dto.Files.Count == 0) return BadRequest("No files uploaded.");

                var incident = await _uow.Incidents.GetByIdAsync(dto.IncidentId);
                if (incident == null) return NotFound("Incident not found.");

                //Folder path tyar karna (wwwroot/uploads/incidents)
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "incidents");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in dto.Files)
                {                    
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName.ToString();
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    
                    string extension = Path.GetExtension(file.FileName.ToString()).ToLower();
                    string mediaType = (extension == ".mp4" || extension == ".mov" || extension == ".avi") ? "Video" : "Image";
                    
                    var media = new IncidentMedia
                    {
                        IncidentId = dto.IncidentId,
                        Url = "/uploads/incidents/" + uniqueFileName,
                        MediaType = mediaType
                    };

                    await _uow.IncidentMedias.AddAsync(media);
                }

                await _uow.CompleteAsync();
                return Ok(new { Message = "Files uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading media");
                return StatusCode(500, "Internal server error.");
            }
        }

        //GET Single Media By ID (Specific file detail)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<IncidentMediaDto>> GetMediaById(Guid id)
        {
            try
            {
                var media = await _uow.IncidentMedias.GetByIdAsync(id);
                if (media == null) return NotFound("Media not found.");

                return Ok(new IncidentMediaDto(media.Id, media.Url, media.MediaType));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching media {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        //GET All Media for a Specific Incident (Gallery View)
        [AllowAnonymous]
        [HttpGet("incident/{incidentId}")]
        public async Task<ActionResult<IEnumerable<IncidentMediaDto>>> GetMediaByIncidentId(Guid incidentId)
        {
            try
            {                
                var mediaList = await _uow.IncidentMedias.FindAsync(m => m.IncidentId == incidentId);

                var result = mediaList.Select(m => new IncidentMediaDto(m.Id, m.Url, m.MediaType));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching media for incident {incidentId}");
                return StatusCode(500, "Internal server error.");
            }
        }

        //PUT Purani file ko DELETE kar ke Nayi file lagana (Update)
        [HttpPut("{mediaId}")]
        public async Task<IActionResult> UpdateMedia(Guid mediaId, Microsoft.AspNetCore.Http.IFormFile newFile)
        {
            try
            {
                var existingMedia = await _uow.IncidentMedias.GetByIdAsync(mediaId);
                if (existingMedia == null) return NotFound();
                
                string oldFilePath = Path.Combine(_env.WebRootPath, existingMedia.Url.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "incidents");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + newFile.FileName.ToString();
                string newPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await newFile.CopyToAsync(stream);
                }
                
                existingMedia.Url = "/uploads/incidents/" + uniqueFileName;
                existingMedia.MediaType = Path.GetExtension(newFile.FileName.ToString()).ToLower().Contains("mp4") ? "Video" : "Image";
                existingMedia.UpdatedAt = DateTime.UtcNow;

                _uow.IncidentMedias.Update(existingMedia);
                await _uow.CompleteAsync();

                return Ok(new { Message = "File updated and old one deleted physically" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating media");
                return StatusCode(500, "Update failed.");
            }
        }

        //DELETE Soft Delete File server par rahegi par dashboard se gayab ho jayegi
        [HttpDelete("{mediaId}")]
        public async Task<IActionResult> SoftDeleteMedia(Guid mediaId)
        {
            try
            {
                var media = await _uow.IncidentMedias.GetByIdAsync(mediaId);
                if (media == null) return NotFound();
                
                _uow.IncidentMedias.Delete(media);
                await _uow.CompleteAsync();

                return Ok(new { Message = "Media soft-deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media");
                return StatusCode(500, "Delete failed.");
            }
        }
    }
}