using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public abstract class ISceneData
{
    [SerializeField]
    protected GameObject RootObj;

    [SerializeField]
    protected string bgmName;

    public string BgmName => bgmName;

    public virtual void Init() {
        if (RootObj) RootObj.SetActive(false);
    }

    public virtual void Start() {
        if (RootObj) RootObj.SetActive(true);
    }

    public virtual void End() {
        if (RootObj) RootObj.SetActive(false);
    }
}

public abstract class ISceneState : MonoBehaviour
{
    public UnityEvent OnSceneInit, OnSceneBegin, OnSceneEnd;

    public int Stage { get; protected set; }

    public virtual UniTask Init() {
        OnSceneInit?.Invoke();

        return UniTask.CompletedTask;
    }

    public virtual void StartScene() {
        OnSceneBegin?.Invoke();
    }

    public virtual void Update() {
    }

    public virtual void Shutdown() {
        OnSceneEnd?.Invoke();

        GameObject.Destroy(gameObject);
    }

    public virtual async UniTask ChangeScene(string sceneName) {
        await SceneManager.LoadSceneAsync(sceneName);
    }
}