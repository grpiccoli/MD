using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ConsultaMD.Hubs
{
    public class FeedBackHub : Hub
    {
        public async Task Send2User(string user, string message)
        {
            await Clients.User(user).SendAsync("FeedBack", message).ConfigureAwait(true);
        }
        public async Task Send2Connection(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("FeedBack", message).ConfigureAwait(true);
        }
    }
}
