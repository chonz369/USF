using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DataTableManager : IGameSystem
{
    public static DataTableManager Instance { get; private set; }

    private IDataTableHelper dataTableHelper;

    private Dictionary<string, DataTableBase> dataTables;

    public DataTableConfiguration Configuration { get; }

    public const string DataRowClassPrefixName = "DR";

    public DataTableManager(DataTableConfiguration config) {
        Configuration = config;
    }

    public UniTask InitializeServiceAsync() {
        Instance = this;

        dataTables = new Dictionary<string, DataTableBase>();

        SetDataTableHelper(new DefaultDataTableHelper());

        return UniTask.CompletedTask;
    }

    public void ResetService() {
    }

    public void DestroyService() {
        dataTables.Clear();

        //dtLoader?.ReleaseAll(this);
    }

    public void SetDataTableHelper(IDataTableHelper dataTableHelper) {
        this.dataTableHelper = dataTableHelper;
    }

    public virtual async UniTask LoadDataTable(string dataTableName) {
        Debug.Log("Load datatable : " + dataTableName);

        var path = FilePathUtil.Combine(DataRowClassPrefixName + dataTableName);
        var handle = Addressables.LoadAssetAsync<TextAsset>(path);
        var dtResource = await handle;

        var dataTableClassName = string.Concat(DataRowClassPrefixName, dataTableName);
        var dataRowType = Type.GetType(dataTableClassName);

        Type dataTableType = typeof(DataTable<>).MakeGenericType(dataRowType);
        DataTableBase dataTableBase = (DataTableBase)Activator.CreateInstance(dataTableType);
        dataTableBase.SetDataTableHelper(dataTableHelper);
        dataTableBase.ParseData(handle.Result.text);

        dataTables.Add(dataTableName, dataTableBase);

        Addressables.Release(handle);
    }

    public bool HasDataTable(string dataTableName) {
        return dataTables.ContainsKey(dataTableName);
    }

    public DataTable<T> GetDataTable<T>() where T : DataRowBase, new() {
        var key = typeof(T).ToString().StartsWith(DataRowClassPrefixName) ?
            typeof(T).ToString().Substring(2) : typeof(T).ToString();
        if (!dataTables.ContainsKey(key)) {
            return null;
        }

        return (DataTable<T>)dataTables[key];
    }

    public void UnloadDataTable(string dataTableName) {
        if (!HasDataTable(dataTableName)) return;

        dataTables.Remove(dataTableName);
    }

    public void UnloadDataTable<T>() where T : DataRowBase {
        var key = typeof(T).ToString().StartsWith(DataRowClassPrefixName) ?
            typeof(T).ToString().Substring(2) : typeof(T).ToString();

        if (!HasDataTable(key)) return;

        dataTables.Remove(key);
    }

    public override UniTask Init() {
        return UniTask.CompletedTask;
    }

    public class DataTable<T> : DataTableBase where T : DataRowBase, new()
    {
        private Dictionary<int, T> dataRows;

        public T GetData(int index) {
            if (!dataRows.ContainsKey(index)) return null;

            return dataRows[index];
        }

        public T[] GetAllData() {
            T[] arr = new T[dataRows.Count];
            dataRows.Values.CopyTo(arr, 0);
            return arr;
        }

        public override void ParseData(string data) {
            dataRows = new Dictionary<int, T>();

            dataTableHelper.ParseData(this, data);
        }

        public override bool AddDataRow(string rowString) {
            T row = new T();

            if (!row.ParseDataRow(rowString)) {
                return false;
            }
            dataRows.Add(row.Id, row);

            return true;
        }

        public override int Count => dataRows.Count;

        public override bool HasData(int i) {
            return dataRows.ContainsKey(i);
        }
    }
}