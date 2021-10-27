using System;

namespace AudioBoos.Data.Persistence.Interfaces; 

public interface IBaseEntity {
    public Guid Id { get; set; }
}