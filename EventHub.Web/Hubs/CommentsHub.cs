using Microsoft.AspNetCore.SignalR;

namespace EventHub.Web.Hubs
{
    public class CommentsHub : Hub
    {
        public async Task SendComment(int eventId, string user, string message)
        {
            await Clients.All.SendAsync("ReceiveComment", eventId, user, message);
        }
    }
}