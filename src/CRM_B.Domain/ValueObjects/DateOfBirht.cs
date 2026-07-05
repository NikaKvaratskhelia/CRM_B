using System;

namespace CRM_B.Domain.ValueObjects;

public record DateOfBirth
{
    public DateTime Value { get; init; }

    public DateOfBirth(DateTime value)
    {
        if (value > DateTime.UtcNow)
        {
            throw new ArgumentException("დაბადების თარიღი მომავალში ვერ იქნება");
        }

        Value = value;
    }
}