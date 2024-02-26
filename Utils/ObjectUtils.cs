using UnityEngine;

namespace USF.Utils
{
    public static class ObjectUtils
    {
        public static bool ExistsInScene<T>() where T : Component
        {
            return Object.FindObjectsOfType<T>().Length > 1;
        }
    }
}