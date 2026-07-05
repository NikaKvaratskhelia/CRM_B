using System;

namespace CRM_B.Domain.ValueObjects;

public record PhoneNumber
{
    public string Value { get; init; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("ტელეფონის ნომერი ცარიელი ვერ იქნება");
        }

        Value = value;
    }
}