using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BootUI : CustomUI
{
    [SerializeField]
    private FadeTextureStream fadeTextureStream;

    [SerializeField]
    private UnityEvent onBootFinished;

    public override void Open(ISimpleUI prevUI = null) {
        base.Open(prevUI);

        StartCoroutine(BootProcess());
    }

    private IEnumerator BootProcess() {
        yield return null;//other's awake

        fadeTextureStream.Play();

        yield return new WaitWhile(() => fadeTextureStream.IsPlaying);

        onBootFinished?.Invoke();

        Close();
    }
}