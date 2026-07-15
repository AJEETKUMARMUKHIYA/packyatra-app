using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Services
{
    public class TicketAttachmentService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public TicketAttachmentService(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        public async Task<TicketAttachment> UploadAsync(int ticketId, IFormFile file)
        {
            var folderPath = Path.Combine(
                _env.ContentRootPath,
                "Uploads",
                "Tickets",
                ticketId.ToString()
            );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var storedFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(folderPath, storedFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new TicketAttachment
            {
                TicketId = ticketId,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                FilePath = fullPath,
                FileSize = file.Length,
                UploadedOn = DateTime.Now
            };

            _context.TicketAttachment.Add(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public List<TicketAttachment> GetByTicket(int ticketId)
        {
            // Remove ?? since UploadedOn is non-nullable
            return _context.TicketAttachment
                .Where(x => x.TicketId == ticketId)
                .OrderByDescending(x => x.UploadedOn)
                .ToList();
        }

        public TicketAttachment GetById(int attachmentId)
        {
            return _context.TicketAttachment.FirstOrDefault(x => x.AttachmentId == attachmentId);
        }
    }
}
