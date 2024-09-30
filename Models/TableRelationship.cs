namespace PgsqlManagementApp;

public class TableRelationship
{
    public string ConstraintName { get; set; }
    public string TableFrom { get; set; }
    public string FromColumn { get; set; }
    public string TableTo { get; set; }
    public string ToColumn { get; set; }
}