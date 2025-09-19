# AppLovin MAX Unity Demo

A Unity sample project demonstrating a clean, maintainable integration of **AppLovin MAX** for showing ads:
- Multiple Banner, Interstitial, and Rewarded placements
- ScriptableObject profile to configure everything in the Inspector (no code edits)
- Tiny, single-purpose controllers (per ad type) with safe callback unsubscribe
- A button helper that triggers ads by label

![AppLovin MAX Unity Demo Screenshot in Unity Editor](images/applovin-demo-app.png)

[Test the app on your own (APK Build 1.2.2)](https://drive.google.com/file/d/1OzIHEC8nNAk1F-vSLpD3YsqLrOG33_F7/view?usp=sharing)

[Watch the app in action (Video Recording)](https://drive.google.com/file/d/1BYvfu6HTYCzlI3IkeQhTREfwakFt5mkN/view?usp=sharing)

The project implements **all listener and delegate methods** for each ad type — even if only for logging — so you can clearly see the lifecycle of ads and how to hook into them.

This project and documentation can be extended upon request. New features like Zenject and Reactive demonstration, automated android version or similar useful implementations can be added.

---

## Features

### Ad Types Implemented
- **Banner** – Toggle on/off, positioned via settings.
- **Interstitial** – Fullscreen ad between content.
- **Rewarded A & Rewarded B** – Two separate reward scenarios for testing.

### Listener Coverage

All relevant MAX callbacks are implemented and logged:

- **SDK**
  - `OnSdkInitializedEvent`
- **Banner**
  - `OnAdLoadedEvent`
  - `OnAdLoadFailedEvent`
  - `OnAdClickedEvent`
  - `OnAdExpandedEvent`
  - `OnAdCollapsedEvent`
  - `OnAdRevenuePaidEvent`
- **Interstitial**
  - `OnAdLoadedEvent`
  - `OnAdLoadFailedEvent`
  - `OnAdDisplayedEvent`
  - `OnAdDisplayFailedEvent`
  - `OnAdClickedEvent`
  - `OnAdHiddenEvent`
  - `OnAdRevenuePaidEvent`
- **Rewarded**
  - `OnAdLoadedEvent`
  - `OnAdLoadFailedEvent`
  - `OnAdDisplayedEvent`
  - `OnAdDisplayFailedEvent`
  - `OnAdClickedEvent`
  - `OnAdHiddenEvent`
  - `OnAdReceivedRewardEvent`
  - `OnAdRevenuePaidEvent`

---

## Quick Start

1. Create a Profile

    Right-click in Project: Create → Ads → MAX Ads Profile

2. Add items to:

    * Banners: set label, adUnitId, anchor, background, startHidden

    * Interstitials: set label, adUnitId, preloadOnStart

    * Rewardeds: set label, adUnitId, preloadOnStart, optional rewardKey (e.g., coins, revive) and fallbackAmount

    The profile validates duplicates and empty fields in OnValidate (warnings in Console).

3. Drop a Bootstrap Prefab in Your First Scene

    * Create an empty GameObject called MaxAdsBootstrap

* Add:

    * MaxSdkBootstrap (SDK init; optional verbose logging and test GAID)

    * MaxAds (assign your MaxAdsProfile)

    Both components are designed to live across scenes (via DontDestroyOnLoad [Singleton] in scripts).

4. Add Buttons (Optional)

    Add a Button and attach MaxAdsButton

    * Set Action (e.g., ShowRewarded), Label (must match a profile label), and (optionally) drag a reference to your MaxAds (or let it auto-find).
    * **This script is helpful for you to check how to call the desired ads in an event driven way.**


That’s it—press Play and use the buttons. Logs will show lifecycle events.

---

## Public API Structure and Script Descriptions

* Banners
    
        MaxAds.ShowBanner("Home_Banner_1");
        MaxAds.HideBanner("Home_Banner_1");
        MaxAds.ToggleBanner("Home_Banner_1");

* Interstitials
        
        MaxAds.ShowInterstitial("LevelEnd_Interstitial");
        MaxAds.PreloadInterstitial("LevelEnd_Interstitial");

* Rewardeds
        
        MaxAds.ShowRewarded("Revive_Rewarded");
        MaxAds.PreloadRewarded("Revive_Rewarded");

### Reward Routing (Optional, Built-In)

MaxAds listens to each RewardedAds controller’s OnRewardGranted event and calls a tiny router:

If rewardKey == "coins" → ResourceA.ReceiveReward(fallbackAmount)

If rewardKey == "revive" → ResourceB.ReceiveReward(fallbackAmount)

---

## Controllers (What They Do)

* BannerAds

    Creates banner with AdViewConfiguration (anchor / background), safe unsubscribe of callbacks

    * Show, 
    * Hide, 
    * Toggle, 

* InterstitialAds

    * Initialize, 
    * Preload, 
    * Show

**Exponential backoff retry on load failures (1→64s)**

**Re-preloads after Hidden / DisplayFailed**

* RewardedAds

**Same lifecycle as Interstitials**

* Raises OnRewardGranted(label) on OnAdReceivedReward

All controllers use named handlers for MAX callbacks and unsubscribe in OnDestroy, preventing event buildup after script reloads.

---

## How It Works

1. **[Initialization](Assets/Scripts/Max/MaxSdkBootstrap.cs)**
   - `MaxSdkBootstrap` runs at startup.

2. **[Ad Service](Assets/Scripts/Max/MaxAds.cs)**
   - `MaxAds` creates and initializes controllers for 
        - Banner, 
        - Interstitial, 
        - Rewarded A, and Rewarded B.
   - Exposes simple methods: 
        - `ToggleBanner()`, 
        - `ShowInterstitial()`, 
        - `ShowRewardedA()`, 
        - `ShowRewardedB()`.

3. **Controllers**
   - Each ad type has its own controller: 
        - [BannerAdController](Assets/Scripts/Max/BannerAds.cs), 
        - [InterstitialAdController](Assets/Scripts/Max/InterstitialAds.cs), 
        - [RewardedAdController](Assets/Scripts/Max/RewardedAds.cs).
   - Subscribes to **all** MAX events and logs them.
   - Includes retry logic for failed loads (Interstitial & Rewarded).
   - Banner appearance and placement is configurable via [MaxAdProfile](Assets/Scripts/Max/MaxAdsProfile.cs).

4. **Rewards**
   - [ResourcesController](Assets/Scripts/Resources/ResourcesController.cs) listens to `OnAdReceivedRewardEvent`.
   - Matches ad unit IDs to **ResourceA** or **ResourceB**.
   - Grants rewards (with fallback amounts if `reward.Amount` is 0).
   - [ResourceLabel](Assets/Scripts/UI/ResourceLabel.cs) updates UI automatically when resource values change.

5. **UI Binding**
   - [AdButtonsBinder](Assets/Scripts/UI/AdButtonsBinder.cs) connects Unity UI Buttons to ad service methods once `MaxAdsService` is ready.
   - [MediationButton](Assets/Scripts/UI/MediationButton.cs) opens the Mediation Debugger.

---

## Debugging Tools

- **Mediation Debugger** – Use the “Open Mediation Debugger” button to inspect network setup.
- **Verbose Logging** – Toggle in `MaxInitializer` to see detailed logs.
- **Event Logs** – All ad lifecycle events are logged with `[Banner]`, `[Interstitial]`, `[Rewarded]` tags.

---

## Known Issues

There is no guard to protect the current Ad Service implementation from pausing/unfocusing the app. There may occur unwanted issues accordingly.

---

## Automated Versioning

This repo includes automated versioning and continuous integration setup, so commit on main branch triggers version bumping without the need for manual actions.

* Automated Versioning – Uses semantic-release to bump version numbers, generate changelogs, and tag releases based on commit history.

- Why it matters – Ensures that demo builds shared with clients are always reproducible, versioned, and match the code in the repo.

## License

This sample is for demonstration purposes.

**Logo / Branding Notice:**  
The application icon and any sample branding used in this demo **do not belong to me**. They are included **solely to improve UX and presentation** for demonstration purposes. All trademarks, service marks, and logos remain the property of their respective owners. No affiliation or endorsement is implied. If you are a rights holder and would like any asset removed, please contact me and I will take it down immediately.

## Author & Asset Attribution

**Developer:** Alp Kurt, Berlin, Germany

krtalp@gmail.com  

If you have questions about this demo or the implementation details, feel free to reach out.

