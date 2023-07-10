using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public abstract class CustomUI : MonoBehaviour, ISimpleUI
{
    protected CanvasGroup canvasGroup;

    [SerializeField]
    protected bool visibleOnAwake = false, controlInteract = true;

    [SerializeField]
    protected bool pauseOnOpen = false;

    public bool PauseOnOpen => pauseOnOpen;

    public ISimpleUI PrevUI { get => prevUI; }

    [SerializeField]
    protected ISimpleUI prevUI;

    public UnityEvent onOpen, onClose, onEndOpen, onEndClose;

    [SerializeField]
    protected float fadeDuration;

    [SerializeField]
    protected AudioClip bgm;

    public AudioClip Bgm {
        get { return bgm; }
        set { bgm = value; }
    }

    [SerializeField]
    private AudioClipEvent playBgmEvent;

    [SerializeField]
    private float bgmIntroTime;

    [SerializeField]
    private bool stopBgmIfNone;

    public bool IsOpened => canvasGroup.alpha > 0.999f;
    public bool IsClosed => canvasGroup.alpha < 0.001f;
    public bool IsVisible => canvasGroup.alpha >= 0.001f;

    protected virtual void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();

        SetVisible(visibleOnAwake);
    }

    protected void SetVisible(bool visible) {
        canvasGroup.alpha = visible ? 1 : 0;

        if (controlInteract) {
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }

    protected virtual void Start() {
    }

    public virtual void Open(ISimpleUI prevUI) {
        this.prevUI = prevUI;

        this.gameObject.SendMessage("OnOpen", SendMessageOptions.DontRequireReceiver);
        onOpen.Invoke();

        ChangeBgm();

        StartCoroutine(CoOpening());
    }

    public virtual void Open() {
        Open(null);
    }

    protected virtual IEnumerator CoOpening() {
        if (controlInteract)
            canvasGroup.blocksRaycasts = false;

        if (fadeDuration > 0.001f)
            yield return canvasGroup.DOFade(1, fadeDuration).WaitForCompletion();

        SetVisible(true);

        onEndOpen?.Invoke();

        this.gameObject.SendMessage("OnEndOpen", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void Close() {
        this.gameObject.SendMessage("OnClose", SendMessageOptions.DontRequireReceiver);
        onClose.Invoke();

        StartCoroutine(CoClosing());
    }

    protected virtual IEnumerator CoClosing() {
        if (controlInteract)
            canvasGroup.blocksRaycasts = false;

        if (fadeDuration > 0.001f)
            yield return canvasGroup.DOFade(0, fadeDuration).WaitForCompletion();

        SetVisible(false);

        onEndClose?.Invoke();

        this.gameObject.SendMessage("OnEndClose", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void Back() {
        Close();

        if (prevUI != null) prevUI.Open(prevUI.PrevUI);
    }

    protected virtual void ChangeBgm() {
        if (Bgm) {
            playBgmEvent.Raise(Bgm);
        }
    }

    protected virtual void OnDestroy() {
        DOTween.Kill(canvasGroup);
    }
}