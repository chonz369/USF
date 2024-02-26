using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;

//using Unity.Services.RemoteConfig;
using UnityEngine;
using USF.Core;

public class AdmobSystem : MonoBehaviour, GameSystem
{
    //[SerializeField]
    //private TitleUI titleUI;

    //[SerializeField]
    //private StageSelectUI stageSelectUI;

    //[SerializeField]
    //private MainGameUI mainGameUI;

    //[SerializeField]
    //private ResultUI resultUI;

    private BannerView bottomBanner, middleBanner;
    //private InterstitialAd interstitial;

    //private int interstitialRequestTimes;
    //private int interstitialRequestTimeThreshold = 3;

    //private bool interstitialAdIsShowed;

    //[SerializeField]
    //private bool debugClose, debugCloseReturnResult = true;

    private RewardedAd rewardedAd;

    public static AdmobSystem Instance { get; private set; }

    private string bottomBannerUnitId;

    //private string middleBannerUnitId, interstitialId;
    private HashSet<string> middleAdHolders = new HashSet<string>();

    public UniTask Init(GameEngine gameEngine) {
        InitAdUnitIds();

        return UniTask.CompletedTask;
    }

    private void InitAdUnitIds() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
        bottomBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_ANDROID
            bottomBannerUnitId = "ca-app-pub-6062844440376273/8251269425";//little cedar games andriod bottom banner
#elif UNITY_IOS
            bottomBannerUnitId = "ca-app-pub-6062844440376273/1127705293";//little cedar games ios bottom banner
#else
            bottomBannerUnitId = "unexpected_platform";
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
        //middleBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_ANDROID
            middleBannerUnitId = "ca-app-pub-6062844440376273/8251269425";//little cedar games andriod bottom banner
#elif UNITY_IOS
            middleBannerUnitId = "ca-app-pub-6062844440376273/1127705293";//little cedar games ios bottom banner
#else
            middleBannerUnitId = "unexpected_platform";
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
        //interstitialId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_ANDROID
            string interstitialAdId = "ca-app-pub-6062844440376273/3873040710";//little cedar games's interstitial
#elif UNITY_IOS
            string interstitialAdId = "ca-app-pub-6062844440376273/9702870326";///little cedar games's ios interstitial
#else
            string interstitialAdId = "unexpected_platform";
#endif
    }

    private void Awake() {
        Instance = this;

        //UnityAnalyticsManager.Instance.OnGetRemoteConfig += OnGetRemoteConfig;

        //ConfigManager.Instance.OnRemoveAD += ClearBanner;
    }

    //private void OnGetRemoteConfig(RuntimeConfig config) {
    //    interstitialRequestTimeThreshold = config.GetInt("InterstitialRequestTimeThreshold", 3);
    //}

    private void Start() {
        //InitRewardedAD();

        //MobileAds.Initialize(initStatus => InitializeComplete(initStatus));
    }

    //    private void InitRewardedAD() {
    //#if UNITY_EDITOR || UNITY_STANDALONE_WIN || DEVELOPMENT_BUILD
    //        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
    //#elif UNITY_ANDROID
    //            string adUnitId = "ca-app-pub-6062844440376273/4421815955";//little cedar games's rewarded
    //#elif UNITY_IOS
    //            string adUnitId = "ca-app-pub-6062844440376273/3136638535";///little cedar games's ios rewarded
    //#else
    //            string adUnitId = "unexpected_platform";
    //#endif
    //        this.rewardedAd = new RewardedAd(adUnitId);
    //        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

    //        LoadRewardedAd();
    //    }

    //    private void HandleRewardedAdClosed(object sender, EventArgs e) {
    //        LoadRewardedAd();
    //    }

    //    private void LoadRewardedAd() {
    //        AdRequest request = new AdRequest.Builder().Build();
    //        rewardedAd.LoadAd(request);
    //    }

    //    public async UniTask<bool> RequestRewardedVideo() {
    ////#if UNITY_EDITOR
    ////        if (debugClose) return debugCloseReturnResult;
    ////#endif

    //#if !UNITY_IOS && !UNITY_ANDROID
    //            return false;
    //#endif

    //        if (!rewardedAd.IsLoaded()) {
    //            LoadRewardedAd();

    //            return false;
    //        }

    //        bool adEnded = false;
    //        bool result = false;
    //        rewardedAd.OnAdFailedToShow += OnFailed;
    //        rewardedAd.OnUserEarnedReward += OnSucceeded;

    //        rewardedAd.Show();

    //        await UniTask.WaitUntil(() => adEnded);

    //        rewardedAd.OnAdFailedToShow -= OnFailed;
    //        rewardedAd.OnUserEarnedReward -= OnSucceeded;

    //        return result;

    //        void OnFailed(object sender, AdErrorEventArgs e) {
    //            adEnded = true;
    //            result = false;

    //            InfoUI.Instance.DisplayMessage("AD Failed :" + e.ToString());
    //        }

    //        void OnSucceeded(object sender, Reward e) {
    //            adEnded = true;
    //            result = true;

    //            InfoUI.Instance.DisplayMessage("Thanks for watching!");
    //        }
    //    }

    private void InitializeComplete(InitializationStatus initStatus) {
        Debug.Log("Admob init complete : " + initStatus.ToString());
    }

    public void ShowInterstitial() {
        //interstitialRequestTimes++;

        //if (interstitial != null) {
        //    if (interstitialAdIsShowed) {
        //        interstitial.Destroy();
        //        interstitial = null;
        //        interstitialAdIsShowed = false;
        //    } else {
        //        if (interstitial.CanShowAd() && interstitialRequestTimes >= interstitialRequestTimeThreshold) {
        //            interstitialAdIsShowed = true;
        //            interstitial.Show();
        //            interstitialRequestTimes = 0;
        //        }

        //        return;
        //    }
        //}

        //// Initialize an InterstitialAd.
        //AdRequest request = new AdRequest();

        //InterstitialAd.Load(interstitialId, request, (InterstitialAd ad, LoadAdError error) => {
        //    // if error is not null, the load request failed.
        //    if (error != null || ad == null) {
        //        Debug.LogError("interstitial ad failed to load an ad " +
        //                       "with error : " + error);

        //        if (interstitial != null) {
        //            interstitial.Destroy();
        //            interstitial = null;
        //        }
        //        interstitialAdIsShowed = false;
        //        return;
        //    }

        //    Debug.Log("Interstitial ad loaded with response : "
        //              + ad.GetResponseInfo());

        //    interstitial = ad;
        //});

        //interstitialAdIsShowed = false;
    }

    [SerializeField]
    private Vector2Int bannerSize = new Vector2Int(468, 60);

    [SerializeField]
    private Vector2Int bannerPosition;

    public void ShowBannerAd(int id) {
        LoadBanner(bottomBannerUnitId, new AdSize(bannerSize.x, bannerSize.y), bannerPosition, ref bottomBanner);
    }

    public void HideBannerAd() {
        bottomBanner?.Destroy();
    }

    //public void ShowMiddleBannerAd(string holder) {
    //    middleAdHolders.Add(holder);

    //    LoadBanner(middleBannerUnitId, AdSize.MediumRectangle, AdPosition.Center, ref middleBanner);
    //}

    private void LoadBanner(string unitId, AdSize adSize, Vector2Int position, ref BannerView bannerView) {
        if (bannerView != null && !bannerView.IsDestroyed) {
            return;
        }

        bannerView = new BannerView(unitId, adSize, position.x, position.y);

        AdRequest request = new AdRequest();

        var _bannerView = bannerView;
        bannerView.OnBannerAdLoadFailed += x => OnBannerFailedToLoad(_bannerView, x);

        bannerView.LoadAd(request);

        void OnBannerFailedToLoad(BannerView bannerView, LoadAdError e) {
            if (bannerView != null) {
                bannerView.Destroy();
            }
        }
    }

    public void HideMiddleBannerAd(string holder) {
        middleAdHolders.Remove(holder);

        if (middleAdHolders.Count == 0)
            middleBanner?.Destroy();
    }
}