using System.Text.RegularExpressions;


namespace InnoShop.Users.Domain.ValueObjects;


public readonly struct Email
{
    private static readonly Regex Rx =
        new(@"^[^@\n\r]+@[^@\n\r]+\.[^@\n\r]+$", RegexOptions.Compiled);
    public string Value { get; }
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Rx.IsMatch(value))
            throw new ArgumentException("Invalid email", nameof(value));
        Value = value;
    }
    public override string ToString() => Value;
}