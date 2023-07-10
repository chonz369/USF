using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager : IGameSystem
{
    private Canvas RootCanvas;

    private Dictionary<string, CustomUI> uiDictionary;

    public static UIManager Instance { get; private set; }

    private AssetLoader<GameObject> uiLoader;

    private HashSet<CustomUI> pausingUIs;

    public override UniTask Init() {
        Instance = this;

        uiLoader = new AssetLoader<GameObject>();
        uiDictionary = new Dictionary<string, CustomUI>();
        pausingUIs = new HashSet<CustomUI>();

        RootCanvas = FindObjectOfType<UICanvasRoot>().Canvas;

        foreach (var item in RootCanvas.GetComponentsInChildren<CustomUI>()) {
            item.onOpen.AddListener(() => OnUIOpen(item));
            item.onClose.AddListener(() => OnUIClose(item));

            uiDictionary.Add(item.GetType().ToString(), item);
        }

        return UniTask.CompletedTask;
    }

    private void OnUIOpen(CustomUI ui) {
        if (ui.PauseOnOpen) {
            pausingUIs.Add(ui);

            Time.timeScale = 0;
        }
    }

    private void OnUIClose(CustomUI ui) {
        if (ui.PauseOnOpen) {
            pausingUIs.Remove(ui);

            if (pausingUIs.Count == 0) {
                Time.timeScale = 1;
            }
        }
    }

    private void Awake() {
        //foreach (var item in RootCanvas.GetComponentsInChildren<ISimpleUI>()) {
        //    item.Init();
        //}
    }

    public async UniTask<T> LoadUIAsync<T>() where T : ISimpleUI {
        var uiName = typeof(T).ToString().TrimStart('I');
        var g = await uiLoader.LoadAndHoldAsync("Assets/Prefabs/UI/" + uiName, this);
        g.transform.parent = RootCanvas.transform;

        var ui = g.GetComponent<CustomUI>();

        uiDictionary.Add(uiName, ui);

        return ui.GetComponent<T>();
    }

    public void RemoveUI(string uiName) {
        if (!uiDictionary.ContainsKey(uiName)) {
            Debug.LogWarning("remove unexist ui : " + uiName);
            return;
        }

        var ui = uiDictionary[uiName];
        uiDictionary.Remove(uiName);

        uiLoader.UnloadAsset("Assets/Prefabs/UI/" + uiName, this);

        Addressables.ReleaseInstance(ui.gameObject);
    }

    public T GetUI<T>() where T : ISimpleUI {
        var uiName = typeof(T).ToString().TrimStart('I');

        return uiDictionary[uiName].GetComponent<T>();
    }
}