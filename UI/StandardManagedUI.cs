using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using USF.Core;

namespace USF.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class StandardManagedUI : MonoBehaviour, ManagedUI
    {
        private CanvasGroup canvasGroup;

        protected UISystem uiSystem;

        [SerializeField]
        private bool closeOnLoad = true;

        private MMF_Player currentFeedback;

        public UnityEvent OnOpen => onOpen;
        public UnityEvent OnClose => onClose;

        [SerializeField]
        private UnityEvent onOpen, onClose;

        [SerializeField]
        private MMF_Player openFeedback, closeFeedback;

        [SerializeField]
        private bool visibleOnAwake = false;

        [SerializeField]
        private bool pauseOnOpen = false;

        [SerializeField]
        private bool setCanvasInteractableOnVisible = true, controlCanvasAlpha = true;

        public bool CloseOnLoad => closeOnLoad;

        /// <summary>
        /// This is true as soon as Open() is called, and is false when Close() is called.
        /// </summary>
        public bool IsOpen {
            get => isOpen;
            private set {
                isOpen = value;
            }
        }

        private bool isOpen;

        protected bool SkipTransition {
            get => skipTransition;
            private set {
                skipTransition = value;
            }
        }

        private bool skipTransition;

        public bool PauseOnOpen => pauseOnOpen;

        public bool IsMajor { get => isMajor; }

        public ManagedUI PrevUI { get => prevUI; }
        private ManagedUI prevUI;

        /// <summary>
        /// Only one major ui can be active at any time also it should not be destroyed/removed.
        /// </summary>
        [SerializeField]
        private bool isMajor = true;

        [SerializeField]
        private bool debugCheckState = true;

        public virtual void Back() {
            Close();

            if (PrevUI != null) PrevUI.Open();
        }

        public virtual void Close(bool skipTransition = false) {
            CloseAsync(skipTransition).Forget();
        }

        public virtual UniTask Init(GameEngine gameEngine) {
            uiSystem = gameEngine.GetSystem<UISystem>();

            return UniTask.CompletedTask;
        }

        public virtual void Open(ManagedUI prevUI = null, bool skipTransition = false) {
            OpenAsync(prevUI, skipTransition).Forget();
        }

        public async UniTask CloseAsync(bool skipTransition = false) {
            if (!IsOpen) {
                if (debugCheckState)
                    Debug.LogWarning($"Close is called on unopen ui {gameObject.name}.");
                return;
            }

            IsOpen = false;
            OnClose?.Invoke();
            SkipTransition = skipTransition;

            SetCanvasReceiveInput(false);

            await LateCloseAsync(skipTransition);
        }

        protected virtual async UniTask LateCloseAsync(bool skipTransition) {
            if (skipTransition) {
                if (closeFeedback != null)
                    closeFeedback.ShouldRevertOnNextPlay = false;
            } else {
                await PlayFeedback(closeFeedback);
            }

            SetCanvasVisible(false);
        }

        public virtual async UniTask OpenAsync(ManagedUI prevUI, bool skipTransition = false) {
            if (IsOpen) {
                if (debugCheckState)
                    Debug.LogWarning($"Open is called on opened ui {gameObject.name}.");
                return;
            }

            this.prevUI = prevUI;

            IsOpen = true;
            SkipTransition = skipTransition;

            OnOpen.Invoke();

            SetCanvasReceiveInput(false);

            await LateOpenAsync(skipTransition);
        }

        protected virtual void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();

            SetCanvasVisible(visibleOnAwake);
            SetCanvasReceiveInput(visibleOnAwake);
        }

        protected virtual async UniTask LateOpenAsync(bool skipTransition) {
            if (skipTransition) {
                openFeedback.ShouldRevertOnNextPlay = false;
            } else {
                await PlayFeedback(openFeedback);
            }

            SetCanvasVisible(true);
            SetCanvasReceiveInput(true);
        }

        protected virtual async UniTask PlayFeedback(MMF_Player feedback) {
            if (feedback == null) {
                return;
            } else {
                currentFeedback = feedback;

                await UniTask.WaitUntil(() => Time.frameCount > 2);//playing too early results wrong "isPlaying" of feedback

                await feedback.PlayFeedbacksTask(Vector3.zero);
            }
        }

        protected void SetCanvasReceiveInput(bool receive) {
            canvasGroup.blocksRaycasts = receive;
        }

        protected virtual void SetCanvasVisible(bool visible, bool? blockRaycast = null) {
            if (controlCanvasAlpha)
                canvasGroup.alpha = visible ? 1 : 0;

            if (setCanvasInteractableOnVisible) {
                canvasGroup.interactable = visible;
            }
        }

        protected virtual void Start() {
        }
    }
}