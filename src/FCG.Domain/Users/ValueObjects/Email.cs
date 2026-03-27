using System.Text.RegularExpressions;
using FCG.Domain.Users.Exceptions;

namespace FCG.Domain.Users.ValueObjects;

public sealed class Email
{
    private static readonly Regex Regex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(250));

    public string Address { get; }

    private Email(string address) => Address = address;

    public static Email FromStorage(string raw) => new(raw);

    public static Email Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new UserDomainException("E-mail address cannot be null or empty.");

        return !Regex.IsMatch(address.Trim())
            ? throw new UserDomainException($"E-mail address '{address}' has an invalid format.")
            : new Email(address.Trim().ToLowerInvariant());
    }

    public override bool Equals(object? obj) =>
        obj is Email other && string.Equals(Address, other.Address, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Address);

    public override string ToString() => Address;

    public static implicit operator string(Email email) => email.Address;
    public static implicit operator Email(string address) => Create(address);
}