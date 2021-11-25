using System;

namespace AudioBoos.Data.Interfaces;

public interface IBaseAudioEntity {
    //Ignore errors here, Rider needs to catch up with preview features
    static abstract bool IsIncomplete(IBaseAudioEntity audioEntity);
    Guid Id { get; set; }
}
