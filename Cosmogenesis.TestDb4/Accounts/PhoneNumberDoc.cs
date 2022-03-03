using Cosmogenesis.Core.Attributes;

namespace Cosmogenesis.TestDb4.Accounts;

[Transient]
[Mutable]
public sealed class PhoneNumberDoc : AccountDocBase
{
    public static string GetId(string phoneNumberType) => $"PhoneNumberType={phoneNumberType}";

    public string PhoneNumberType { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public bool IsActive { get; set; }
}
