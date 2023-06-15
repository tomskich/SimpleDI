namespace SimpleDI.Test.Services;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}