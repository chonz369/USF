using UnityEngine;

namespace USF.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UICanvasRoot : MonoBehaviour
    {
        private Canvas canvas;

        public Canvas Canvas {
            get {
                if (canvas == null) {
                    canvas = GetComponent<Canvas>();
                }
                return canvas;
            }
        }
    }
}