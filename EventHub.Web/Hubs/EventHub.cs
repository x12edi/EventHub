using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EventHub.Web.Hubs
{
    public class EventHubb : Hub
    {
        public async Task SendEventNotification(string title)
        {
            await Clients.All.SendAsync("ReceiveEventNotification", title);
        }
    }
}