using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace AlejoF.Contacts
{
    public class ContactSenderFunction
    {
        private readonly IMediator _mediator;

        public ContactSenderFunction(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [FunctionName("SendContact")]
        public async Task Run(
            [QueueTrigger(Constants.DefaultQueueName)]Models.SubmissionData queueItem, ILogger log,
            [SendGrid]IAsyncCollector<SendGridMessage> emailCollector)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueItem.Id}");

            var request = new Handlers.FormContacts.Request { Data = queueItem };
            var result = await _mediator.Send(request);

            if (result.EmailMessage != null)
                await emailCollector.AddAsync(result.EmailMessage);
        }
    }
}
