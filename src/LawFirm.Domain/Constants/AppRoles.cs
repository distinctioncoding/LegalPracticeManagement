namespace LawFirm.Domain.Constants;

public static class AppRoles
{
    public const string SystemAdmin = "SystemAdmin";
    public const string Partner = "Partner";
    public const string Lawyer = "Lawyer";
    public const string Paralegal = "Paralegal";
    public const string AdminStaff = "AdminStaff";

    public static readonly List<string> All =
    [
        SystemAdmin, Partner, Lawyer, Paralegal, AdminStaff
    ];
}
