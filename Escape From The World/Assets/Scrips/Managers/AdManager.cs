using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

#if UNITY_ANDROID
    string adBannerID = "ca-app-pub-3940256099942544/6300978111"; //Test: ca-app-pub-3940256099942544/6300978111  Reel:ca-app-pub-3885973286274777/2209361050
    string adInterstitialID = "ca-app-pub-3940256099942544/1033173712"; //Test: ca-app-pub-3940256099942544/1033173712  Reel:ca-app-pub-3885973286274777/5645171049
    string adRewardedID = "ca-app-pub-3940256099942544/1033173712"; //Test: ca-app-pub-3940256099942544/1033173712  Reel:ca-app-pub-3885973286274777/9975298294
#elif UNITY_IPHONE 
        //Ýlerde
#else

#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                return;
            }
            LoadInterstitialAd();
            LoadRewardedAd();
        });
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; //banner ana menüde sürekli çalýþsýn diye kontrol ediyoruz
    }

    //Sürekli Oynatýlacak
    #region AdBanner
    public void LoadAd()
    {
            if (_bannerView != null)
            {
                DestroyAd();
            }
            _bannerView = new BannerView(adBannerID, AdSize.Banner, AdPosition.Top);

        var adRequest = new AdRequest();
        _bannerView.LoadAd(adRequest);
    }

    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
   
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) //banner ana menüde sürekli çalýþsýn diye kontrol ediyoruz
    {
        if (scene.buildIndex == 0)
        {
            LoadAd();
        }
        else
        {
            DestroyAd();
        }
    }
    #endregion

    //2 Ölmede bir Oynatýlacak
    #region AdInterstitial
    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        InterstitialAd.Load(adInterstitialID, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                //Yüklenemedi
                return;
            }
            //Yüklendi
            _interstitialAd = ad;
            _interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                //LoadInterstitialAd(); //Reklam kapatýldýðýnda tekrardan yeni reklamý yükleyecek
            };
            _interstitialAd.OnAdFullScreenContentFailed += (error) =>
            {
                Debug.LogError("Geçiþ reklamý gösterilirken hata oluþtu: " + error.GetMessage());
                LoadInterstitialAd();
            };
        });
    }

    public void ShowIntertitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Geçiþ Reklamý Yüklenemedi");
            LoadInterstitialAd();
        }
    }
    #endregion


    //5 Ölmede bir Oynatýlacak
    #region AdReward

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        RewardedAd.Load(adRewardedID, new AdRequest(),(RewardedAd ad, LoadAdError error) => 
        {
            if (error != null || ad == null)
            {
                return;
            }
            _rewardedAd = ad;
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                //LoadRewardedAd();
            };
            _rewardedAd.OnAdFullScreenContentFailed += (error) =>
            {
                Debug.LogError("Ödüllü reklam gösterilirken hata oluþtu: " + error.GetMessage());
                LoadRewardedAd();
            };
        });
    }

    public void ShowRewardedAd()
    {
        if (_rewardedAd != null &&_rewardedAd.CanShowAd())
        {
            _rewardedAd.Show ((Reward reward) =>
            {
                Debug.Log($"User earned Reward: {reward.Amount} {reward.Type}");
            });
        }
        else
        {
            LoadRewardedAd();
        }
    }
    #endregion
}
