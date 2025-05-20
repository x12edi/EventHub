using EventHub.Application.Services;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Web.Components;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EventHub.Web.Tests
{
    public class EventServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly IEventService _eventService;

        public EventServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEventRepository = new Mock<IEventRepository>();
            _mockUnitOfWork.Setup(u => u.Events).Returns(_mockEventRepository.Object);
            _eventService = new EventService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddAsync_ValidEvent_SetsSlugAndSaves()
        {
            // Arrange
            var evt = new Event
            {
                Id = 1,
                Title = "Test Event",
                Description = "Test Description",
                StartDate = new DateTime(2025, 5, 10),
                Organizer = "Test Organizer",
                IsActive = true
            };
            _mockEventRepository.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(1));

            // Act
            await _eventService.AddAsync(evt);

            // Assert
            _mockEventRepository.Verify(r => r.AddAsync(It.Is<Event>(e =>
                e.Id == evt.Id &&
                e.Title == evt.Title &&
                e.Slug == "test-event-2025")), Times.Once());
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task AddAsync_NullEvent_ThrowsArgumentNullException()
        {
            // Arrange
            Event evt = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _eventService.AddAsync(evt));
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsEvent()
        {
            // Arrange
            var evt = new Event
            {
                Id = 1,
                Title = "Test Event",
                Description = "Test Description",
                StartDate = new DateTime(2025, 5, 10),
                Organizer = "Test Organizer",
                IsActive = true
            };
            _mockEventRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(evt);

            // Act
            var result = await _eventService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(evt.Id, result.Id);
            Assert.Equal(evt.Title, result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockEventRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Event)null);

            // Act
            var result = await _eventService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ValidEvent_SetsSlugAndSaves()
        {
            // Arrange
            var evt = new Event
            {
                Id = 1,
                Title = "Updated Event",
                Description = "Updated Description",
                StartDate = new DateTime(2025, 5, 10),
                Organizer = "Test Organizer",
                IsActive = true
            };
            _mockEventRepository.Setup(r => r.UpdateAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(1));

            // Act
            await _eventService.UpdateAsync(evt);

            // Assert
            _mockEventRepository.Verify(r => r.UpdateAsync(It.Is<Event>(e =>
                e.Id == evt.Id &&
                e.Title == evt.Title &&
                e.Slug == "updated-event-2025")), Times.Once());
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_ExistingEvent_CallsDeleteAndSaves()
        {
            // Arrange
            var evt = new Event { Id = 1, Title = "Test Event" };
            _mockEventRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(evt);
            _mockEventRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.FromResult(1));

            // Act
            await _eventService.DeleteAsync(1);

            // Assert
            _mockEventRepository.Verify(r => r.DeleteAsync(1), Times.Once());
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_NonExistingEvent_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockEventRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Event)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _eventService.DeleteAsync(999));
        }

        [Fact]
        public async Task SearchAsync_WithTitleAndStatus_ReturnsFilteredEvents()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Title = "Test Event", IsActive = true },
                new Event { Id = 2, Title = "Other Event", IsActive = false }
            };
            _mockEventRepository.Setup(r => r.SearchAsync("Test", true)).ReturnsAsync(events.Where(e => e.Title.Contains("Test") && e.IsActive));

            // Act
            var result = await _eventService.SearchAsync("Test", true);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Event", result.First().Title);
        }

        [Fact]
        public async Task EventSummaryViewComponent_UsesCache()
        {
            var mockEventService = new Mock<IEventService>();
            var mockCache = new Mock<IMemoryCache>();
            var cacheEntry = new Mock<ICacheEntry>();
            object cachedValue = null;
            mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object)
                     .Callback<object>(key => cachedValue = key);
            mockEventService.Setup(s => s.SearchAsync(null, true)).ReturnsAsync(new List<Event>
            {
                new Event { Id = 1, Title = "Test Event", StartDate = new DateTime(2025, 5, 10), IsActive = true }
            });

            var component = new EventSummaryViewComponent(mockEventService.Object, mockCache.Object);

            await component.InvokeAsync();

            mockCache.Verify(c => c.TryGetValue("EventSummary", out It.Ref<object>.IsAny), Times.Once());
            mockEventService.Verify(s => s.SearchAsync(null, true), Times.Once());

            // Simulate cache hit
            mockCache.Setup(c => c.TryGetValue("EventSummary", out It.Ref<object>.IsAny)).Returns(true);
            await component.InvokeAsync();

            mockEventService.Verify(s => s.SearchAsync(null, true), Times.Once()); // Should not call again
        }
    }
}