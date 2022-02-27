﻿namespace Cosmogenesis.Core.Tests;

public static class EnumHelper<TEnum> where TEnum : Enum
{
    static readonly TEnum[] values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
    public static IEnumerable<TEnum> Values => values.ToArray();
}
