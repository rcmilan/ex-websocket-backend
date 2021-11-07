using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teste_websocket_backend.Models;

namespace teste_websocket_backend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser = "Chat Bot"; // usuário para mensagens automáticas
        private readonly IDictionary<string, UserRoom> _connections; // singleton com as conexões e usuários/salas

        private readonly string _receiveMessageMethod = "ReceiveMessage"; // nome da função/método do frontend   
        private readonly string _usersInRoomMethod = "UsersInRoom"; // nome da função/método do frontend   

        public ChatHub(IDictionary<string, UserRoom> connections)
        {
            _connections = connections;
        }

        public async Task JoinRoom(UserRoom userRoom)
        {
            // adiciona a sala no grupo
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoom.Room);

            // adiciona o usuário/sala no singleton utilizando o connectionId como chave
            _connections[Context.ConnectionId] = userRoom;

            //await Clients.All // envia para todos os clients
            //    .SendAsync(method, _botUser, $"{userConnection.User} has joined {userConnection.Room}");

            await Clients.Group(userRoom.Room) // avisa os clients do grupo que o usuário entrou na sala
                .SendAsync(_receiveMessageMethod, _botUser, $"{userRoom.User} has joined {userRoom.Room}");

            // atualiza usuários na sala
            await SendConnectedUsers(userRoom.Room);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // busca o usuário/sala dentro das conexões registradas
            if (_connections.TryGetValue(Context.ConnectionId, out UserRoom userConnection))
            {
                // remove a conexão do usuário
                _connections.Remove(Context.ConnectionId);

                Clients.Group(userConnection.Room) // avisa os clients do grupo que o usuário saiu
                    .SendAsync(_receiveMessageMethod, _botUser, $"{userConnection.User} has left");

                // atualiza usuários na sala
                SendConnectedUsers(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendConnectedUsers(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room.Equals(room))
                .Select(c => c.User);

            return Clients.Group(room) // avisa os clients do grupo quais são os usuários ainda logados
                .SendAsync(_usersInRoomMethod, users);
        }

        public async Task SendMessage(string message)
        {
            // busca o usuário/sala dentro das conexões registradas
            if (_connections.TryGetValue(Context.ConnectionId, out UserRoom userConnection))
            {
                await Clients.Group(userConnection.Room) // avisa os clients do grupo sobre o usuário e a mensagem
                    .SendAsync(_receiveMessageMethod, userConnection.User, message);
            }
        }
    }
}
