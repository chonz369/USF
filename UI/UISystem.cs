using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using USF.Core;
using USF.Utils;

namespace USF.UI
{
    public interface ManagedUI
    {
        bool CloseOnLoad { get; }

        bool PauseOnOpen { get; }

        ManagedUI PrevUI { get; }

        UniTask Init(GameEngine gameEngine);

        void Close(bool skipTransition = false);

        void Open(ManagedUI prevUI = null, bool skipTransition = false);

        UniTask CloseAsync(bool skipTransition = false);

        UniTask OpenAsync(ManagedUI prevUI = null, bool skipTransition = false);

        UnityEvent OnOpen { get; }
        UnityEvent OnClose { get; }
    }

    public class UISystem : MonoBehaviour, GameSystem
    {
        [SerializeField]
        private UICanvasRoot canvasPrefab;

        private Canvas rootCanvas;

        private Dictionary<string, GameObject> uiDictionary;
        private HashSet<ManagedUI> pausingUIs;
        private AssetLoader<GameObject> uiLoader;

        private GameEngine gameEngine;

        [SerializeField]
        private GameObject[] spawnUIs;

        [SerializeField]
        private GameObject startUIPrefab;

        public T GetUI<T>() where T : ManagedUI {
            return GetUI<T>(typeof(T).ToString());
        }

        public T GetUI<T>(string uiName) where T : ManagedUI {
            uiName = ParseUIName(uiName);

            return uiDictionary.Where(p => p.Key == uiName)
                .Select(p => p.Value.GetComponent<T>())
                .FirstOrDefault();
        }

        public async UniTask Init(GameEngine gameEngine) {
            this.gameEngine = gameEngine;

            uiLoader = new AssetLoader<GameObject>();
            uiDictionary = new Dictionary<string, GameObject>();
            pausingUIs = new HashSet<ManagedUI>();
            InitRootCanvas(gameEngine);

            await SpawnUIs();

            var startUI = await LoadUIAsync(startUIPrefab.GetComponent<ManagedUI>().GetType().ToString());
            startUI.Open();
        }

        private async UniTask SpawnUIs() {
            await UniTask.WhenAll(spawnUIs.Select(x => LoadUIAsync(x.GetComponent<ManagedUI>().GetType().ToString())));
        }

        public async UniTask<ManagedUI> LoadUIAsync(string uiName) {
            return await LoadUIAsync<ManagedUI>(uiName);
        }

        public async UniTask<T> LoadUIAsync<T>() where T : ManagedUI {
            return await LoadUIAsync<T>(typeof(T).ToString());
        }

        public async UniTask<T> LoadUIAsync<T>(string uiName) where T : ManagedUI {
            uiName = ParseUIName(uiName);

            T resultUI = GetUI<T>(uiName);

            if (resultUI != null) {
                return resultUI;
            }

            var prefab = await uiLoader.LoadAndHoldAsync($"Assets/Prefabs/UIs/{uiName}", this);
            var ui = Instantiate(prefab, rootCanvas.transform);
            uiDictionary.Add(uiName, ui);
            var managedUI = ui.GetComponent<ManagedUI>();
            managedUI.OnOpen.AddListener(() => OnUIOpen(managedUI));
            managedUI.OnClose.AddListener(() => OnUIClose(managedUI));

            await ui.GetComponent<ManagedUI>().Init(gameEngine);

            return ui.GetComponent<T>();
        }

        public async UniTask OpenUIAsync<T>(ManagedUI prevUI = null, bool skipTransition = false) where T : ManagedUI {
            var ui = await LoadUIAsync<T>();
            await ui.OpenAsync(prevUI, skipTransition);
        }

        public async UniTask CloseUIAsync<T>(bool skipTransition = false) where T : ManagedUI {
            await GetUI<T>().CloseAsync(skipTransition);
        }

        public void CloseUI<T>(bool skipTransition = false) where T : ManagedUI {
            GetUI<T>().CloseAsync(skipTransition).Forget();
        }

        public void OpenUI<T>(ManagedUI prevUI = null, bool skipTransition = false) where T : ManagedUI {
            GetUI<T>().OpenAsync(prevUI, skipTransition).Forget();
        }

        private string ParseUIName(string uiName) {
            return uiName.Contains(".") ? uiName.GetAfter(".") : uiName;
        }

        private void OnUIClose(ManagedUI ui) {
            if (ui.PauseOnOpen) {
                pausingUIs.Remove(ui);

                if (pausingUIs.Count == 0) {
                    Time.timeScale = 1;
                }
            }
        }

        private void OnUIOpen(ManagedUI ui) {
            if (ui.PauseOnOpen) {
                pausingUIs.Add(ui);

                Time.timeScale = 0;
            }
        }

        private void InitRootCanvas(GameEngine gameEngine) {
            var root = FindFirstObjectByType<UICanvasRoot>();
            if (root != null) {
                rootCanvas = root.Canvas;
            } else {
                //todo : prevent two canvas root

                rootCanvas = Instantiate(canvasPrefab).GetComponent<Canvas>();
            }
            rootCanvas.worldCamera = gameEngine.GetSystem<CameraSystem>().UICamera;
        }

        public void RemoveUI<T>() where T : ManagedUI {
            var uiName = ParseUIName(typeof(T).ToString());
            if (!uiDictionary.ContainsKey(uiName)) {
                Debug.LogWarning("remove unexist ui : " + uiName);
                return;
            }
            var ui = uiDictionary[uiName];
            uiDictionary.Remove(uiName);
            uiLoader.UnloadAsset("Assets/Prefabs/UIs/" + uiName, this);
            GameObject.Destroy(ui.gameObject);
        }

        private void OnDestroy() {
            uiLoader.UnloadAll();
        }
    }
}