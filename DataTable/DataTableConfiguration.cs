public class DataTableConfiguration
{
    public const string DefaultDataTablePathPrefix = "DataTable";
    public const string DtNameTemplate = "{0}/{1}";

    public static string GetDataTablePath(string dtName) {
        return string.Format(DtNameTemplate, dtName);
    }
}