using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using USF.Core;

namespace USF.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class StandardManagedUI : MonoBehaviour, ManagedUI
    {
        private CanvasGroup canvasGroup;

        [SerializeField]
        private bool closeOnLoad = true;

        private MMF_Player currentFeedback;

        [SerializeField]
        private UnityEvent onOpen, onClose, onEndOpen, onEndClose;

        [SerializeField]
        private MMF_Player openFeedback, closeFeedback;

        [SerializeField]
        private bool openOnAwake = false;

        [SerializeField]
        private bool pauseOnOpen = false;

        [SerializeField]
        private StandardManagedUI prevUI;

        [SerializeField]
        private bool setCanvasInteractableOnVisible = true, controlCanvasAlpha = true;

        private Coroutine transitionCoroutine;

        public bool CloseOnLoad => closeOnLoad;

        /// <summary>
        /// This is true as soon as Open() is called, and is false when Close() is called.
        /// </summary>
        public bool IsOpen { get; private set; }

        public bool PauseOnOpen => pauseOnOpen;

        public StandardManagedUI PrevUI { get => prevUI; }

        public virtual void Back()
        {
            Close();

            if (prevUI != null) prevUI.Open(prevUI.PrevUI);
        }

        public void Close()
        {
            Close(skipTransition: false);
        }

        public virtual void Close(bool skipTransition)
        {
            if (!IsOpen) return;

            IsOpen = false;

            OnClose();

            StopCurrentTransition();

            transitionCoroutine = StartCoroutine(CoClosing(skipTransition));
        }

        public void CloseFast()
        {
            Close(skipTransition: true);
        }

        public virtual void Init(GameEngine gameEngine)
        {
        }

        public void Open()
        {
            Open(null, skipTransition: false);
        }

        public void Open(StandardManagedUI prevUI)
        {
            Open(prevUI, false);
        }

        public virtual void Open(StandardManagedUI prevUI, bool skipTransition)
        {
            if (IsOpen) return;

            IsOpen = true;
            if (prevUI != null) this.prevUI = prevUI;

            OnOpen();

            StopCurrentTransition();

            transitionCoroutine = StartCoroutine(CoOpening(skipTransition));
        }

        public void OpenFast()
        {
            OpenFast(null);
        }

        public virtual void OpenFast(StandardManagedUI prevUI)
        {
            Open(prevUI, skipTransition: true);
        }

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            SetCanvasVisible(openOnAwake);
            SetCanvasReceiveInput(openOnAwake);
        }

        protected virtual IEnumerator CoClosing(bool skipTransition)
        {
            SetCanvasReceiveInput(false);

            if (!skipTransition)
                yield return PlayFeedback(closeFeedback);

            SetCanvasVisible(false);

            transitionCoroutine = null;

            OnEndClose();
        }

        protected virtual IEnumerator CoOpening(bool skipTransition)
        {
            SetCanvasReceiveInput(false);

            if (!skipTransition)
                yield return PlayFeedback(openFeedback);

            SetCanvasVisible(true);
            SetCanvasReceiveInput(true);

            transitionCoroutine = null;

            OnEndOpen();
        }

        protected virtual void OnClose()
        {
            onClose.Invoke();
        }

        protected virtual void OnEndClose()
        {
            onEndClose.Invoke();
        }

        protected virtual void OnEndOpen()
        {
            onEndOpen.Invoke();
        }

        protected virtual void OnOpen()
        {
            onOpen.Invoke();
        }

        protected virtual IEnumerator PlayFeedback(MMF_Player feedback)
        {
            if (feedback == null)
            {
                yield break;
            } else
            {
                yield return feedback.PlayFeedbacksCoroutine(Vector3.zero);
            }
        }

        protected void SetCanvasReceiveInput(bool receive)
        {
            canvasGroup.blocksRaycasts = receive;
        }

        protected virtual void SetCanvasVisible(bool visible, bool? blockRaycast = null)
        {
            if (controlCanvasAlpha)
                canvasGroup.alpha = visible ? 1 : 0;

            if (setCanvasInteractableOnVisible)
            {
                canvasGroup.interactable = visible;
            }
        }

        protected virtual void Start()
        {
        }

        protected virtual void StopCurrentTransition()
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);

                transitionCoroutine = null;
            }

            currentFeedback?.StopFeedbacks();
        }
    }
}