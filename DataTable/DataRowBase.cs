public abstract class DataRowBase
{
    public int Id { get; protected set; }

    public abstract bool ParseDataRow(string dataRowString);
}