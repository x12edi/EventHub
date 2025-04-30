Setup Instructions:

mkdir EventHub
cd EventHub
dotnet new classlib -o EventHub.Core
dotnet new classlib -o EventHub.Infrastructure
dotnet new classlib -o EventHub.Application
dotnet new mvc -o EventHub.Web
dotnet new xunit -o EventHub.Web.Tests
dotnet new sln
dotnet sln add EventHub.Core EventHub.Infrastructure EventHub.Application EventHub.Web EventHub.Web.Tests

Install Dependencies:
EventHub.Core: None.
EventHub.Infrastructure: Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Design.
EventHub.Application: Reference EventHub.Core, EventHub.Infrastructure.
EventHub.Web: Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.AspNetCore.SignalR, Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Design.
EventHub.Web.Tests: Moq, Microsoft.AspNetCore.Mvc.Core.


run below command from root of .sln folder:
* dotnet ef migrations add InitialCreate --project EventHub.Infrastructure --startup-project EventHub.Web
* dotnet ef database update --project EventHub.Infrastructure --startup-project EventHub.Web
	conn string format : "Server=localhost;Database=EventHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
-----------
create identity related tables:
dotnet ef migrations add AddIdentity --project EventHub.Infrastructure --startup-project EventHub.Web
dotnet ef database update --project EventHub.Infrastructure --startup-project EventHub.Web



EventHub/
├── EventHub.Core/
│   ├── Entities/
│   ├── Interfaces/
├── EventHub.Infrastructure/
│   ├── Data/
│   ├── Repositories/
├── EventHub.Application/
│   ├── Services/
├── EventHub.Web/
│   ├── Controllers/
│   ├── Views/
│   ├── wwwroot/
│   ├── Filters/
│   ├── Middleware/
│   ├── TagHelpers/
├── EventHub.Web.Tests/


I have sent approval mail to daean , cc to you on Mar-18 and Apr-15.
story completed but no response from daean on it. we are still waiting for his approval!

-----------

I sent the approval email to Daean, with you in CC, on March 18 and April 15. The story has been completed, but we haven't received any response from Daean yet — we're still awaiting his approval.

