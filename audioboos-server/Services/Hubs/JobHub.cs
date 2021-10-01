using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AudioBoos.Server.Services.Hubs {
    public interface IJobMessageClient {
        Task QueueJobMessage(JobMessage message);
    }

    public class JobMessage {
        public string Message { get; set; }
        public int Percentage { get; set; }
    }

    public class JobHub : Hub {
        public Task SendMessage(JobMessage message) {
            return Clients.All.SendAsync("QueueJobMessage", message);
        }
    }
}
