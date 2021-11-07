using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using teste_websocket_backend.Models;

namespace teste_websocket_backend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        public ChatHub()
        {
            _botUser = "Bot User";
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            string method = "ReceiveMessage"; // nome da função/método do frontend            
            
            //await Clients.All // envia para todos os clients
            //    .SendAsync(method, _botUser, $"{userConnection.User} has joined {userConnection.Room}");
            
            await Clients.Group(userConnection.Room) // envia para clients no grupo
                .SendAsync(method, _botUser, $"{userConnection.User} has joined {userConnection.Room}");
        }
    }
}
