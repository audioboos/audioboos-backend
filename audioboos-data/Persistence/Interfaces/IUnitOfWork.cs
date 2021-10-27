using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Persistence.Interfaces; 

public interface IUnitOfWork {
    public DbContextId __contextId();
    public Task<bool> Complete();
}