using System;

namespace CRM_B.Domain.ValueObjects;

public record Email
{
    public string Value { get; init; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
        {
            throw new ArgumentException("არასწორი იმეილის ფორმატი");
        }

        Value = value;
    }
}