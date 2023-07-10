using Cysharp.Threading.Tasks;
using UnityEngine;

public class DevConsoleUI : MonoBehaviour
{
    [SerializeField]
    private GameObject content;

    // Start is called before the first frame update
    private void Start() {
        content.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            content.SetActive(!content.activeSelf);
        }
    }

    public void OnClickSwitchScene(string type) {
        //switch (type) {
        //    case "誕生動畫":
        //    GameEngine.Instance.SwitchTo(new CutScene1()).Forget();
        //    break;

        //    case "滾下山坡遊戲":
        //    GameEngine.Instance.SwitchTo(new GameScene()).Forget();
        //    break;
        //}
    }
}