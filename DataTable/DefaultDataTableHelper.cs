using System;
using UnityEngine;

public class DefaultDataTableHelper : IDataTableHelper
{
    public bool ParseData(DataTableBase dataTable, string dataTableString) {
        try {
            int position = 0;
            string dataRowString = null;
            while ((dataRowString = dataTableString.ReadLine(ref position)) != null) {
                if (dataRowString[0] == ';') {
                    continue;
                }

                if (!dataTable.AddDataRow(dataRowString)) {
                    Debug.LogWarning($"Can not parse data row string '{dataRowString}'.");
                    return false;
                }
            }

            return true;
        } catch (Exception exception) {
            Debug.LogWarning($"Can not parse data table string with exception '{exception.ToString()}'.");
            return false;
        }
    }
}