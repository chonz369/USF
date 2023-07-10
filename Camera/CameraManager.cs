using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    public Camera MainCamera, UICamera;

    private Stack<Camera> camStack = new Stack<Camera>();

    private void Awake() {
        Instance = this;

        PushCamera(MainCamera);
    }

    public void PushCamera(Camera cam) {
        if (camStack.Count > 0)
            camStack.Peek().enabled = false;

        camStack.Push(cam);

        cam.enabled = true;
    }

    public Camera PopCamera() {
        var cam = camStack.Pop();
        cam.enabled = false;

        if (camStack.Count > 0)
            camStack.Peek().enabled = true;

        return cam;
    }
}