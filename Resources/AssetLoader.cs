using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading;

public class AssetLoader<T> where T : Object
{
    private int poolSize;

    private const int DEFAULT_POOL_SIZE = 5;

    public AssetLoader() : this(DEFAULT_POOL_SIZE) {
    }

    public AssetLoader(int poolSize) {
        this.poolSize = poolSize;
    }

    private Dictionary<string, AssetData> assetDictionary = new Dictionary<string, AssetData>();

    private class AssetData
    {
        public T Asset;
        public AsyncOperationHandle<T> Handle;
        public HashSet<object> Holders = new HashSet<object>();

        public void AddHolder(object holder) {
            Holders.Add(holder);
        }

        public void RemoveHolder(object holder) {
            Holders.Remove(holder);
        }
    }

    public async UniTask<T> LoadAsync(string filePath, CancellationToken token = default) {
        return await LoadAndHoldAsync(filePath, null, token);
    }

    public async UniTask<T> LoadAndHoldAsync(string filePath, Object holder, CancellationToken token = default) {
        if (assetDictionary.ContainsKey(filePath)) {
            if (holder != null)
                assetDictionary[filePath].AddHolder(holder);

            return assetDictionary[filePath].Asset;
        }

        if (assetDictionary.Count >= poolSize) {
            string keyToRemove = "";
            foreach (var pair in assetDictionary) {
                if (assetDictionary[pair.Key].Holders.Count == 0) {
                    keyToRemove = pair.Key;
                    break;
                }
            }
            if (keyToRemove != "") {
                UnloadAsset(keyToRemove);
            }
        }

        var handle = Addressables.LoadAssetAsync<T>(filePath);
        await handle;

        if (handle.IsValid()) {
            var asset = new AssetData() {
                Asset = handle.Result,
                Handle = handle
            };
            assetDictionary.Add(filePath, asset);
            if (holder != null)
                asset.AddHolder(holder);
            return handle.Result;
        } else {
            Debug.LogError("Load asset failed : " + filePath);
            return null;
        }
    }

    public void UnloadAsset(T asset, Object holder = null) {
        string k = null;
        foreach (var key in assetDictionary.Keys) {
            if (assetDictionary[key].Asset == asset) {
                k = key;
                if (holder != null)
                    assetDictionary[k].RemoveHolder(holder);

                break;
            }
        }

        if (k == null) {
            Debug.LogError($"UnloadAsset is called on unexisted asset{asset.name}");
            return;
        }

        if (assetDictionary[k].Holders.Count == 0) {
            Addressables.Release(assetDictionary[k].Handle);
            assetDictionary.Remove(k);
        }
    }

    public void UnloadAsset(string filePath, Object holder = null) {
        if (!assetDictionary.ContainsKey(filePath)) {
            Debug.LogError($"UnloadAsset is called on unexisted asset : {filePath}");
            return;
        }

        if (holder != null)
            assetDictionary[filePath].RemoveHolder(holder);

        if (assetDictionary[filePath].Holders.Count == 0) {
            Addressables.Release(assetDictionary[filePath].Handle);
            assetDictionary.Remove(filePath);
        }
    }

    public void UnloadAll() {
        foreach (var key in assetDictionary.Keys) {
            Addressables.Release(assetDictionary[key].Handle);
        }
        assetDictionary.Clear();
    }
}