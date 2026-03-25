using FCG.Domain.Shared;

namespace FCG.Domain.Users.Interfaces;

public interface IUserUnitOfWork : IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserGameLibraryRepository UserGameLibraries { get; }
}