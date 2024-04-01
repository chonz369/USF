using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using USF.Utils;
using System.Linq;

namespace USF.Core
{
    public interface GameSystem
    {
        UniTask Init(GameEngine gameEngine) { return UniTask.CompletedTask; }

        void ShutdownSystem() { }

        void UpdateSystem() { }
    }

    [DefaultExecutionOrder(-1000)]
    public class GameEngine : MonoBehaviour
    {
        private List<GameSystem> gameSystemList;

        [SerializeField]
        private UnityEvent onSystemInitializingStart, onSystemInitializingEnd;

        [SerializeField]
        private GameObject[] spawningSystems;

        private async UniTask Awake() {
            if (ObjectUtils.ExistsInScene<GameEngine>()) {
                Destroy(gameObject);

                return;
            } else {
                DontDestroyOnLoad(gameObject);

                await InitSystems();
            }
        }

        private void Update() {
            gameSystemList.ForEach(x => x.UpdateSystem());
        }

        public T GetSystem<T>() where T : GameSystem {
            return (T)gameSystemList.FirstOrDefault(x => x is T);
        }

        public async UniTask InitSystems() {
            onSystemInitializingStart.Invoke();

            gameSystemList = new List<GameSystem>();

            await spawningSystems.ToList().ForEachAsync(
                async x => {
                    var system = Instantiate(x, transform).GetComponent<GameSystem>();
                    gameSystemList.Add(system);
                    await system.Init(this);
                });

            onSystemInitializingEnd.Invoke();
        }
    }
}