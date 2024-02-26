using UnityEngine;

namespace USF.Utils
{
    public static class TransformUtility
    {
        public static void DestroyAllChildren(this Transform transform) {
            for (int i = transform.childCount - 1; i >= 0; i--) {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}