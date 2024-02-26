using Com.LuisPedroFonseca.ProCamera2D;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using USF.Core;

[DefaultExecutionOrder(-1)]
public class CameraSystem : MonoBehaviour, GameSystem
{
    [SerializeField]
    private Camera mainCamera;

    public Camera MainCamera => mainCamera;

    [SerializeField]
    private Camera uiCamera;

    public Camera UICamera => uiCamera;

    private Vector2Int screenSize;

    [SerializeField]
    private Vector2Int defaultScreenSize = new Vector2Int(960, 540);

    public UniTask Init(GameEngine gameEngine)
    {
        Observable.EveryLateUpdate().Subscribe(x => CheckScreenSizeChanged()).AddTo(this);

        return UniTask.CompletedTask;
    }

    private void LateUpdate()
    {
        CheckScreenSizeChanged();
    }

    private void CheckScreenSizeChanged()
    {
        if (Screen.width != screenSize.x || Screen.height != screenSize.y)
        {
            screenSize = new Vector2Int(Screen.width, Screen.height);

            UpdateCameraRect();
        }
    }

    private void UpdateCameraRect()
    {
        var screenRatio = Screen.width / (float)Screen.height;
        var defaultRatio = defaultScreenSize.x / (float)defaultScreenSize.y;
        if (screenRatio > defaultRatio)
        {
            Vector2 size = new Vector2(1 / (screenRatio / defaultRatio), 1);
            MainCamera.rect = new Rect((1 - size.x) / 2, 0, size.x, size.y);
        } else
        {
            Vector2 size = new Vector2(1, 1 / (defaultRatio / screenRatio));
            MainCamera.rect = new Rect(0, (1 - size.y) / 2, size.x, size.y);
        }
        UICamera.rect = MainCamera.rect;
    }
}