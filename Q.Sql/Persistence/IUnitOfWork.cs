using System.Threading.Tasks;

namespace Q.Sql.Persistence
{
    public interface IUnitOfWork
    {
      void SetAutoDetectChangesEnabled(bool x);
      Task CompleteAsync();        
    }
}