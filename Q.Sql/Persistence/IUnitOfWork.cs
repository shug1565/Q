using System.Threading.Tasks;

namespace Q.Sql.Persistence
{
    public interface IUnitOfWork
    {
      Task CompleteAsync();        
    }
}