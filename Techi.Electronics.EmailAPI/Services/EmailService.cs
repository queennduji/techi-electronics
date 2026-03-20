using Microsoft.EntityFrameworkCore;
using System.Text;
using Techi.Electronics.EmailAPI.Data;
using Techi.Electronics.EmailAPI.Data.Model;
using Techi.Electronics.EmailAPI.Data.Model.Dto;

namespace Techi.Electronics.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            if (cartDto?.CartHeader == null)
                throw new ArgumentException("Invalid cart data");

            var message = BuildCartEmail(cartDto);
            await LogAndEmail(message, cartDto.CartHeader.Email);
        }

        private string BuildCartEmail(CartDto cartDto)
        {
            var header = cartDto.CartHeader;
            var details = cartDto.CartDetails ?? new List<CartDetailsDto>();

            var builder = new StringBuilder();

            builder.AppendLine("<h3>Shopping Cart Summary</h3>");
            builder.AppendLine($"<p><strong>Total:</strong> {header.CartTotal:C}</p>");
            builder.AppendLine("<ul>");

            foreach (var item in details)
            {
                var name = item.Product?.Name ?? "Unknown Product";
                builder.AppendLine($"<li>{name} (Qty: {item.Count})</li>");
            }

            builder.AppendLine("</ul>");

            return builder.ToString();
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            var builder = new StringBuilder();

            builder.AppendLine("<h3>User Registration Successful</h3>");
            builder.AppendLine($"<p><strong>Email:</strong> {email}</p>");
            builder.AppendLine($"<p><strong>Registered At:</strong> {DateTime.Now:MMMM dd, yyyy hh:mm tt}</p>");

            var messageBody = builder.ToString();

            Console.WriteLine($"Logging registration email for: {email}");

            await LogAndEmail(messageBody, "admin@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
