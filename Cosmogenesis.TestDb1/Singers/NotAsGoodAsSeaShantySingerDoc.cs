﻿namespace Cosmogenesis.TestDb1.Singers;

[Transient]
public sealed class NotAsGoodAsSeaShantySingerDoc : SingerDocBase
{
    public static string GetId(string firstName, string lastName) => $"NotSeaShanty: {firstName} {lastName}";
}
