using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Q.Sql.Persistence {
  public class UnitOfWork<T> : IUnitOfWork where T : DbContext {
    private readonly T context;
    public UnitOfWork(T context) => this.context = context;
    public async Task CompleteAsync() => await context.SaveChangesAsync();
  }
}