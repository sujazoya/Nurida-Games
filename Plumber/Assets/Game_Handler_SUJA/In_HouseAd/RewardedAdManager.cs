using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System.Collections;
using System;
using UnityEngine.Events;


public class RewardedAdManager : MonoBehaviour
{   
   
    bool show_ad_as_index;
    private bool showAds;    
    public static bool show_rewarded;
    public static int show_rewarded_onrequest_count;  
    public static RewardedAdManager Instance;
    public RewardedButton[] rewardedButtons;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        StartCoroutine(TryToFetch());       
    }
    IEnumerator TryToFetch()
    {
        yield return new WaitForSeconds(3);
        yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);
        show_rewarded = GoogleSheetHandler.show_rewarded;
        show_ad_as_index = GoogleSheetHandler.show_ad_as_index;
        show_rewarded_onrequest_count = int.Parse(GoogleSheetHandler.show_rewarded_onrequest_count);
        showAds = GoogleSheetHandler.showAds;

        if (show_rewarded == true)
        {
            RequestRewarded();
        }
        
    }
    public static void Initialize(Action<InitializationStatus> initCompleteAction) { }
    #region REWARDED
    public static RewardedAd Rewarded; 
    string rewardedAd_ID1;
    string rewardedAd_ID2;
    string rewardedAd_ID3;

    int totallRewarded = 10;   
    public void RequestRewarded()
    {
        rewardedAd_ID1 = GoogleSheetHandler.g_rewarded1;
        rewardedAd_ID2 = GoogleSheetHandler.g_rewarded2;
        rewardedAd_ID3 = GoogleSheetHandler.g_rewarded3;
        Rewarded = RequestRewardedAd(rewardedAd_ID1);    

    }   
    public RewardedAd RequestRewardedAd(string adUnitId)
    {
        RewardedAd myRewardedAd;       
        myRewardedAd = new RewardedAd(adUnitId);

        myRewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        //myRewardedAd.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        //myRewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;
        myRewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
       
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        myRewardedAd.LoadAd(request);
        return myRewardedAd;
    }
    public  bool IsReadyToShowAd()
    {
        if (Rewarded.IsLoaded())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
   

    public void ShowRewardedAd()
    {
       
        if (!showAds||!show_rewarded)
            return;
        StartCoroutine(WaitAplayRewardedAd());
       
    }    
    IEnumerator WaitAplayRewardedAd()
    {
      
       if(!IsReadyToShowAd()) {
          yield return new WaitUntil(() => IsReadyToShowAd()); 
          Rewarded.Show();
        }
        else
        {
         Rewarded.Show();
        }       
       
    }   
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        //MonoBehaviour.print(
        //    "HandleRewardedAdFailedToLoad event received with message: "
        //                     + args.Message);
        //RequestRewardedAd(CurrentRewardedAd_ID());
    }
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleAdOpened event received");
    }
    //public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    //{
    //    string type = args.Type;
    //    double amount = args.Amount;
    //    //RequestRewardedAd(CurrentRewardedAd_ID());
    //    //uiManager.RewardTheUser();

    //}

    //public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    //{
    //    //RequestRewardedAd(CurrentRewardedAd_ID());
    //    //uiManager.WarnAdClosed();
    //}
   
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //  Debug.Log("Rewarded Video ad loaded successfully");

    }
    #endregion
}