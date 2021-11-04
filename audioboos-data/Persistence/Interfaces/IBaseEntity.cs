using System;

namespace AudioBoos.Data.Persistence.Interfaces;

public interface IBaseEntity {
    //Ignore errors here, Rider needs to catch up with preview features
    static abstract bool IsIncomplete(IBaseEntity entity);
    Guid Id { get; set; }
}
