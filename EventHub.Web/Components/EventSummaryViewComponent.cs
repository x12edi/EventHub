using EventHub.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Web.Components
{
    public class EventSummaryViewComponent : ViewComponent
    {
        private readonly IEventService _eventService;
        private readonly IMemoryCache _cache;

        public EventSummaryViewComponent(IEventService eventService, IMemoryCache memoryCache)
        {
            _eventService = eventService;
            _cache = memoryCache;
        }

        //[ResponseCache(Duration = 300)]
        public async Task<IViewComponentResult> InvokeAsync(int maxEvents=5)
        {
            string cacheKey = "EventSummary";
            if (!_cache.TryGetValue(cacheKey, out var cachedEvents))
            {
                var events = await _eventService.SearchAsync(null, true); // Get active events
                var recentEvents = events
                    .OrderBy(e => e.StartDate)
                    .Take(maxEvents)
                    .ToList();
                //return View(recentEvents);
                cachedEvents = recentEvents;

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _cache.Set(cacheKey, cachedEvents, cacheOptions);
            }
            return View(cachedEvents);
        }
    }
}