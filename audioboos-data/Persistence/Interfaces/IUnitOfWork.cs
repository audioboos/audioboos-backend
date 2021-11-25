using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Interfaces; 

public interface IUnitOfWork {
    public DbContextId __contextId();
    public Task<bool> Complete();
}