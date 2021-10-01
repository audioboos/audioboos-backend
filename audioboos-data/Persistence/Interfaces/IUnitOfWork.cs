using System.Threading.Tasks;

namespace AudioBoos.Data.Persistence.Interfaces {
    public interface IUnitOfWork {
        public Task<bool> Complete();
    }
}
