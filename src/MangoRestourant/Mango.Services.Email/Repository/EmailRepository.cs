using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Model;

namespace Mango.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private DbContextOptions<ApplicationDbContext> dbContext;

        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext )
        {
            this.dbContext = dbContext;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage resultMessage)
        {
            // implemant an email sender or call some other class library 
            EmailLog emailLog = new()
            {
                Email = resultMessage.Email,
                EmailSent = System.DateTime.Now,
                Log = $"Order - {resultMessage.OrderId} has been created successsfuly"
            };

            await using var db = new ApplicationDbContext(dbContext);
            db.EmailLogs.Add(emailLog);
            db.SaveChanges();

        }
    }
}
