# EventHub
EventHub is a modular, monolithic web application built with ASP.NET Core MVC to manage events. It serves as a demo project to demonstrate proficiency in modern .NET development, focusing on clean architecture, best practices, and essential MVC concepts. 

Project Overview
EventHub allows users to register, log in, and manage events (create, read, update, delete). The application enforces authentication, ensuring only authorized users can access event management features. It uses a modular architecture with separated layers for presentation, business logic, and data access, showcasing patterns like Repository and Service Layer.
Features Completed

User Authentication:
User registration and login using ASP.NET Core Identity.
Password validation and error handling (e.g., duplicate email, weak password).
Automatic redirect to login page for unauthenticated users accessing protected routes.
Logout functionality.
Header navigation with user info (e.g., "Welcome, user@email.com") and login/logout buttons.

Event Management (CRUD):
List Events: Displays all events in a dynamic table using DataTables.js for sorting, searching, and pagination.
Create Events: Form to create events with validation (e.g., required title, description).
Edit Events: Form to update event details, pre-populated with existing data and validation.
Delete Events: Delete events with a confirmation prompt (JavaScript) to prevent accidental deletion.
Events are tied to the logged-in user (organizer set to user’s email).

UI and UX:
Responsive layout using Bootstrap 5.
Client-side validation with jQuery Validation and Unobtrusive Validation.
Consistent navigation via a shared layout (_Layout.cshtml) with dynamic login/logout buttons.

Architecture
EventHub follows a modular, layered architecture to ensure maintainability and scalability:

EventHub.Web: Presentation layer (ASP.NET Core MVC).
Controllers (AccountController, EventsController).
Razor Views for login, registration, and event management.
Shared layout for consistent UI.

EventHub.Application: Service layer.
IEventService and EventService handle business logic (e.g., setting organizer, validation checks).
Decouples controllers from data access.

EventHub.Infrastructure: Data access layer.
ApplicationDbContext (EF Core) with Events and Identity tables.
IEventRepository and EventRepository for CRUD operations using EF Core.

EventHub.Core: Core entities and interfaces.
Event entity with validation attributes (e.g., [Required]).
Interfaces (IEventRepository, IEventService) for dependency injection.

Technologies Used
Backend: ASP.NET Core 8.0, C#, Entity Framework Core
Frontend: Razor Views, Bootstrap 5, jQuery, DataTables.js, jQuery Validation
Database: SQL Server (via EF Core)
Authentication: ASP.NET Core Identity
Patterns: Repository, Service Layer, Dependency Injection

Setup Instructions
Prerequisites

.NET 8.0 SDK
SQL Server (LocalDB or full instance)
Visual Studio 2022 or VS Code with C# extension
