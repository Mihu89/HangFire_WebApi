using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace HangFire_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HangFireController : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public IActionResult Welcome()
        {
            //job Fire and Forget
            var jobId = BackgroundJob.Enqueue(() => SendWelcomeEmail("Welcome to our App"));
            return Ok($"Job Id: {jobId}. Welcome email was send to user.");
        }

        public void SendWelcomeEmail(string message)
        {
            Console.WriteLine("Title: {0}", message);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Discount()
        {
            // scheduled job (job with delay on start

            int timeDelayInSeconds = 35;
            var jobId = BackgroundJob.Schedule(() => SendWelcomeEmail("You are lucky and have a discount from us"), TimeSpan.FromSeconds(timeDelayInSeconds));
            return Ok($"JobId: {jobId}");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult DatabaseUpdate()
        {
            // periodically job

            RecurringJob.AddOrUpdate(() => Console.WriteLine("Database was updated"), Cron.Minutely);
            return Ok("Database Cron job is working periodically");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Confirmation()
        {
            // jobs Flow
            int timeInSecconds = 20;
            var parentJob = BackgroundJob.Schedule(() => Console.WriteLine("You are in process of unsubscribing from our service"), TimeSpan.FromSeconds(timeInSecconds));
            BackgroundJob.ContinueJobWith(parentJob, () => Console.WriteLine("Your email was removed from our list"));
            return Ok("Confirmation is processing");
        }
    }
}