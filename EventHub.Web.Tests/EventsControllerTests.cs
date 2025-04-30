using EventHub.Application.Services;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventHub.Web.Tests
{
    public class EventsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithEvents()
        {
            var repository = new Mock<IEventRepository>();
            var service = new Mock<IEventService>();
            service.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Event> { new Event { Id = 1, Title = "Test" } });

            var controller = new EventsController(repository.Object, service.Object);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Event>>(viewResult.Model);
            Assert.Single(model);
        }
    }
}