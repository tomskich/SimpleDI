namespace SimpleDI.Test.Services;

public abstract class Repository : IRepository
{
    protected Repository(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public IUnitOfWork UnitOfWork { get; }
}