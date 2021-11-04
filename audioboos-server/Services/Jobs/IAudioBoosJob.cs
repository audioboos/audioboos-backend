using Quartz;

namespace AudioBoos.Server.Services.Jobs; 

public interface IAudioBoosJob : IJob {
    public string JobName { get; }
}