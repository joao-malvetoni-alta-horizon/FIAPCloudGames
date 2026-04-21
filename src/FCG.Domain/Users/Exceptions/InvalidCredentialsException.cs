using FCG.Domain.Games.Exceptions;

namespace FCG.Domain.Users.Exceptions;

public sealed class InvalidCredentialsException() : DomainException("Invalid credentials.");