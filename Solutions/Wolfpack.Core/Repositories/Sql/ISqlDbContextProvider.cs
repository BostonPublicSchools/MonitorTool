
namespace Wolfpack.Core.Repositories.Sql
{
    public interface ISqlDbContextProvider
    {
        ISqlDbContext Provide();
    }
}
