public class DRGameText : DataRowBase
{
    public string AssetName { get; private set; }

    public override bool ParseDataRow(string dataRowString) {
        string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
        for (int i = 0; i < columnStrings.Length; i++) {
            columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
        }

        int index = 0;
        index++;
        Id = int.Parse(columnStrings[index++].Trim());
        index++;
        AssetName = columnStrings[index++];
        return true;
    }
}