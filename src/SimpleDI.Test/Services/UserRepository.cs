namespace SimpleDI.Test.Services;

public class UserRepository : Repository, IUserRepository
{
    public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}