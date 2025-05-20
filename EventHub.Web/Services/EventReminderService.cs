using EventHub.Application.Services;
using EventHub.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace EventHub.Web.Services
{
    public class EventReminderService : BackgroundService
    {

        //private readonly IEventService _eventService; 
        //InvalidOperationException: Error while validating the service descriptor 'ServiceType: Microsoft.Extensions.Hosting.IHostedService Lifetime: Singleton ImplementationType: EventHub.Web.Services.EventReminderService': Cannot consume scoped service 'EventHub.Application.Services.IEventService' from singleton 'Microsoft.Extensions.Hosting.IHostedService'.
        //The error occurs because the EventReminderService, registered as a singleton IHostedService, is trying to consume a scoped service (IEventService). In ASP.NET Core, singletons cannot directly depend on scoped services due to lifetime mismatches, as scoped services are created per request or scope, while singletons persist for the application's lifetime.
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EventReminderService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        //public EventReminderService(IEventService eventService, IConfiguration configuration, ILogger<EventReminderService> logger)
        //{
        //    _eventService = eventService;
        //    _configuration = configuration;
        //    _logger = logger;
        //}
        public EventReminderService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<EventReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventReminderService started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending event reminders.");
                }
                await Task.Delay(_checkInterval, stoppingToken);
            }
            _logger.LogInformation("EventReminderService stopped.");
        }

        private async Task SendRemindersAsync()
        {
            //ASP.NET Core prevents a singleton from holding a scoped dependency to avoid capturing a scope that may be disposed.
            //Solution: Use IServiceScopeFactory to create a new scope within EventReminderService when accessing IEventService, ensuring the scoped service is resolved correctly.
            using var scope = _scopeFactory.CreateScope();
            var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

            var now = DateTime.UtcNow;
            var reminderWindowStart = now.AddHours(23);
            var reminderWindowEnd = now.AddHours(25);

            //var events = await _eventService.SearchAsync(null, true);
            var events = await eventService.SearchAsync(null, true);
            var upcomingEvents = events
                .Where(e => e.StartDate >= reminderWindowStart && e.StartDate <= reminderWindowEnd)
                .ToList();

            if (!upcomingEvents.Any())
            {
                _logger.LogInformation("No events requiring reminders.");
                return;
            }

            foreach (var evt in upcomingEvents)
            {
                await SendReminderEmailAsync(evt);
            }
        }

        private async Task SendReminderEmailAsync(Event evt)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                using var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
                {
                    Credentials = new System.Net.NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"], "EventHub"),
                    Subject = $"Reminder: {evt.Title} is Tomorrow!",
                    Body = $@"Dear User, 
Your event '{evt.Title}' is scheduled for tomorrow, {evt.StartDate:MMMM d, yyyy} at {evt.StartDate:HH:mm}.
Details: {evt.Description}
Organizer: {evt.Organizer}
Join us at: https://yourapp.com/Events/Detail/{evt.Slug}
Thank you,
EventHub Team",
                    IsBodyHtml = false
                };

                // Replace with actual user emails (e.g., from event registrations)
                mailMessage.To.Add("user@example.com");

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Reminder sent for event: {Title} (ID: {Id})", evt.Title, evt.Id);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Failed to send reminder for event: {Title} (ID: {Id})", evt.Title, evt.Id);
            }
        }
    }
}