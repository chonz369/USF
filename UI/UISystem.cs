using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using USF.Core;

namespace USF.UI
{
    public interface ManagedUI
    {
        bool CloseOnLoad { get; }

        bool PauseOnOpen { get; }

        void Close();

        void Init(GameEngine gameEngine);

        void Open();
    }

    public class UISystem : MonoBehaviour, GameSystem
    {
        [SerializeField]
        private UICanvasRoot canvasPrefab;

        private Canvas rootCanvas;

        [SerializeField]
        private GameObject[] spawningUIs;

        [SerializeField]
        private GameObject startUI;

        private Dictionary<string, GameObject> uiDictionary;

        private AssetLoader<GameObject> uiLoader;

        public static UISystem Instance { get; private set; }

        public void CloseAllUIs()
        {
            var uiToClose = uiDictionary.Values.ToList()
                .Select(x => x.GetComponent<ManagedUI>())
                .Where(x => x.CloseOnLoad);

            foreach (var ui in uiToClose)
            {
                ui.Close();
            }
        }

        public T GetUI<T>() where T : ManagedUI
        {
            var uiName = typeof(T).ToString();

            if (!uiDictionary.ContainsKey(uiName))
            {
                return default(T);
            }

            return uiDictionary[uiName].GetComponent<T>();
        }

        public ManagedUI GetUI(string uiName)
        {
            return uiDictionary[uiName].GetComponent<ManagedUI>();
        }

        public UniTask Init(GameEngine gameEngine)
        {
            Instance = this;
            uiLoader = new AssetLoader<GameObject>();
            uiDictionary = new Dictionary<string, GameObject>();
            //pausingUIs = new HashSet<IManagedUI>();
            InitRootCanvas(gameEngine);

            SpawnUIs(gameEngine);

            if (startUI != null)
                GetUI(startUI.GetComponent<ManagedUI>().GetType().Name).Open();

            return UniTask.CompletedTask;
        }

        public async UniTask<T> LoadUIAsync<T>() where T : ManagedUI
        {
            var uiName = typeof(T).ToString();

            var prefab = await uiLoader.LoadAndHoldAsync($"Assets/Prefabs/UIs/{uiName}", this);

            var ui = Instantiate(prefab, rootCanvas.transform);

            uiDictionary.Add(uiName, ui);

            return ui.GetComponent<T>();
        }

        //        if (pausingUIs.Count == 0) {
        //            Time.timeScale = 1;
        //        }
        //    }
        //}
        public void RemoveUI<T>() where T : ManagedUI
        {
            var uiName = typeof(T).ToString();
            if (!uiDictionary.ContainsKey(uiName))
            {
                Debug.LogWarning("remove unexist ui : " + uiName);
                return;
            }

            var ui = uiDictionary[uiName];
            uiDictionary.Remove(uiName);

            uiLoader.UnloadAsset("Assets/Prefabs/UI/" + uiName, this);

            GameObject.Destroy(ui.gameObject);
        }

        private void InitRootCanvas(GameEngine gameEngine)
        {
            var root = FindObjectOfType<UICanvasRoot>();
            if (root != null)
            {
                rootCanvas = root.Canvas;
            } else
            {
                //todo : prevent two canvas root

                rootCanvas = Instantiate(canvasPrefab).GetComponent<Canvas>();
            }
            rootCanvas.worldCamera = gameEngine.GetSystem<CameraSystem>().UICamera;
        }

        private void OnDestroy()
        {
            uiLoader.UnloadAll();
        }

        private void SpawnUIs(GameEngine gameEngine)
        {
            for (int i = 0; i < spawningUIs.Length; i++)
            {
                var item = Instantiate(spawningUIs[i], rootCanvas.transform);
                //item.onOpen.AddListener(() => OnUIOpen(item));
                //item.onClose.AddListener(() => OnUIClose(item));
                var managedUI = item.GetComponent<ManagedUI>();
                managedUI.Init(gameEngine);
                uiDictionary.Add(managedUI.GetType().ToString(), item);
            }
        }

        //private void OnUIClose(ManagedUI ui) {
        //    if (ui.PauseOnOpen) {
        //        pausingUIs.Remove(ui);
        //    }
        //}
        //private HashSet<IManagedUI> pausingUIs;
        //private void OnUIOpen(IManagedUI ui) {
        //    if (ui.PauseOnOpen) {
        //        pausingUIs.Add(ui);

        //        Time.timeScale = 0;
        //    }
        //}
    }
}