
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace HireSphereApi.core.services
{
    public class VideoCallHub : Hub
    {
        // מיפוי בין userId ל-ConnectionId
        private static readonly Dictionary<string, string> _userConnections = new();


        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                _userConnections.Remove(userId);
                Console.WriteLine($"❌ User disconnected: {userId} ({Context.ConnectionId})");
            }
            else
            {
                Console.WriteLine($"❌ Unknown client disconnected: {Context.ConnectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // שלב 1 – רישום המשתמש
        public Task RegisterUser(string userId)
        {
            _userConnections[userId] = Context.ConnectionId;
            Console.WriteLine($"✅ User registered: {userId} => {Context.ConnectionId}");
            return Task.CompletedTask;
        }

        // שלב 2 – שליחת הזמנה לראיון
        public async Task InviteToInterview(string targetUserId, string callerUserId)
        {
            Console.WriteLine("in invite to inrweview");
            if (_userConnections.TryGetValue(targetUserId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveInterviewInvite", callerUserId);
                Console.WriteLine($"📨 Interview invite sent to {targetUserId} from {callerUserId}");
            }
            else
            {
                Console.WriteLine($"❌ User {targetUserId} not connected.");
            }
        }

        // שלב 3 – הצעת שיחה
        public async Task SendOffer(string targetUserId, string offerJson)
        {
            Console.WriteLine("in send offre function");
            if (_userConnections.TryGetValue(targetUserId, out var connectionId))
            {
                Console.WriteLine("inside if in sendoffer function");
                var callerUserId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
                Console.WriteLine(callerUserId + " caller use id");
                Console.WriteLine($"📤 Offer sent from {callerUserId} to {targetUserId}");

                // שליחה כ-JSON
                await Clients.Client(connectionId).SendAsync("ReceiveOffer", callerUserId, offerJson);
            }
            else
            {
                Console.WriteLine($"❌ User  something want worng {targetUserId} not connected.");
            }

        }    
        // שלב 4 – תשובה להצעה
        public async Task SendAnswer(string targetUserId, string answer)
        {
            Console.WriteLine("in send answer function");

            if (_userConnections.TryGetValue(targetUserId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
            }
        }

        public async Task SendIceCandidate(string targetUserId, JsonElement candidate)
        {
            Console.WriteLine("📡 in SendIceCandidate");
            Console.WriteLine(candidate.ToString());

            if (_userConnections.TryGetValue(targetUserId, out var connectionId))
            {
                Console.WriteLine(candidate);
                var callerUserId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
                await Clients.Client(connectionId).SendAsync("ReceiveIceCandidate", callerUserId, candidate);
            }
            else
            {
                Console.WriteLine($"❌ targetUserId {targetUserId} not connected.");
            }
        }



    }
}
