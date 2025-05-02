using EventHub.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Components
{
    public class EventSummaryViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Event evt)
        {
            // Format data for display
            var model = new
            {
                Title = evt.Title,
                FormattedDate = evt.StartDate.ToString("MMMM dd, yyyy"),
                Organizer = evt.Organizer,
                IsActive = evt.IsActive
            };
            return View(model);
        }
    }
}