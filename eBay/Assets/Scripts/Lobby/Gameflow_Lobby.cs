using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Gameflow_Lobby : MonoBehaviour
{
    [Header("Settings")]
    public uint passcode = 8736;
    public float gameDuration = 300;
    public float timeWindowForUserSetup = 15; // the duration of the counter after game starts for the user to select the amount of objects to use on this instance of the game
    public bool usingPasscodeNumbers = false;
    public float videoDelayWithAnnouncementSound = 2;

    [Header("Components")]
    public GameObject[] winScreens;
    public GameObject[] loseScreens;
    public VideoPlayer videoPlayer_TV;
    public VideoPlayer videoPlayer_LobbyTV;
    [Header("Gameflows")]
    public Gameflow_TV TV;
    public Gameflow_Selfcheckout Selfcheckout;

    [Header("Videos")]
    public VideoClip introVideo;
    public IntroVideo[] introVideos_ItemCountRelated;
    public VideoClip allScannedVideo;
    public VideoClip allScannedVideo_Loop;
    public VideoClip allScannedAndFinalVideo_Loop;
    public VideoClip loseVideo;
    public VideoClip winVideo;
    public VideoClip idle_LobbyTV_Video;

    [Header("Audio")]
    public AudioClip SFX_IncomingVideo;
    public AudioClip SFX_error;

    [System.Serializable]
    public struct IntroVideo
    {
        public SettingsScreen.ItemCountOptions itemCount;
        public VideoClip video;
    }

    public VideoClip GetIntroVideo()
    {
        if (introVideos_ItemCountRelated == null || introVideos_ItemCountRelated.Length <= 0) { return null; }
        for (int i = 0; i < introVideos_ItemCountRelated.Length; i++)
        {
            if (introVideos_ItemCountRelated[i].itemCount == SettingsScreen.itemCountOptions) { return introVideos_ItemCountRelated[i].video; }
        }
        return null;
    }

    public void PlayErrorSFX()
    {
        if (!GAME.gameHasEnded && GAME.gameHasStarted)
        {
            Audio.PlaySFX(SFX_error);
        }
    }

    bool lastItemScanned = false;

    // Start is called before the first frame update
    void Start()
    {
        TV.usingPasscodeNumbers = usingPasscodeNumbers;
        Audio.DestroyAllSounds();
        SettingsScreen.ApplyUserSettings();
        Setup();
        videoPlayer_LobbyTV.PlayVideo(idle_LobbyTV_Video, true);
        videoPlayer_TV.PlayVideo(idle_LobbyTV_Video, true);

        TV.OnAllItemsChecked = () =>
        {
            if (lastItemScanned) { GameCompleted(); }
            else
            {
                PlayVideo(allScannedVideo, () => this.ActionAfterFrameDelay(1, () =>
                {
                    if (!lastItemScanned) { videoPlayer_TV.PlayVideo(allScannedVideo_Loop, true); }
                }));
            }
        };
        SERIAL.onMessageReceived = RFID_Scan;
        SERIAL.onMessageReceived += TV.RFID_Scan;
        SERIAL.onMessageReceived += Selfcheckout.RFID_Scan;
    }

    public void StartGame()
    {
        if (!GAME.gameHasStarted)
        {
            videoPlayer_LobbyTV.Close();
            lastItemScanned = false;
            Setup(SettingsScreen.GetSettings_ItemCount());
            //PlayVideo(introVideo, () => this.ActionAfterFrameDelay(1, () => videoPlayer_TV.PlayVideo(GetIntroVideo())));
            PlayVideo(GetIntroVideo());
            GAME.StartGame();
            GAME.timer.OnTimerEnds = LoseGame;
            UDP.Write(GAME.UDP_SetGameTimer + ((int)SettingsScreen.GetSettings_TimerDuration()).ToString());
            UDP.Write(GAME.UDP_GameStart);
        }
    }

    void Setup(int itemCount = -1)
    {
        if (winScreens != null)
        {
            for (int i = 0; i < winScreens.Length; winScreens[i++].SetActive(false)) ;
        }
        if (loseScreens != null)
        {
            for (int i = 0; i < loseScreens.Length; loseScreens[i++].SetActive(false)) ;
        }
        itemCount = itemCount < 0 ? ItemDatabase.Count : Mathf.Min(itemCount, ItemDatabase.Count);
        UDP.Write("SETUP_OBJECTCOUNT_" + itemCount);
        TV.Setup(passcode, itemCount);
        Selfcheckout.Setup();
    }

    private void Update()
    {
        KeyboardCommands();
    }

    enum ItemCountKeyCodes
    {
        AllItems = -1,
        ThreeItems = KeyCode.Alpha3,
        SixItems = KeyCode.Alpha6,
        NineItems = KeyCode.Alpha9
    }

    int itemDebugRevealIndex = 0;
    void KeyboardCommands()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && videoPlayer_TV.IsPlaying)
        {
            videoPlayer_TV.Close(true);
        }

        if (!GAME.gameHasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {

            Item[] items = ItemDatabase.AllItems;
            if (itemDebugRevealIndex < items.Length)
            {
                SERIAL.onMessageReceived(items[itemDebugRevealIndex++].tagID[0]);
            }
        }


        if (Input.GetKeyDown(KeyCode.R) ) { ResetGame(); }
    }

    public void GameCompleted()
    {
        if (!GAME.gameHasEnded)
        {
            GAME.gameHasEnded = true;
            GAME.timer.Pause();
            TV.SetLastItemIncorrectOrderScreenVisibleState(false, null);
            PlayVideo(winVideo, () =>
            {
                UDP.Write(GAME.UDP_GameCompleted);

                this.ActionAfterFrameDelay(1, () =>
                {
                    videoPlayer_TV.PlayVideo(allScannedAndFinalVideo_Loop, true);
                    if (winScreens != null)
                    {
                        for (int i = 0; i < winScreens.Length; winScreens[i++].SetActive(true)) ;
                    }
                });
            });
        }
    }

    public void PlayVideo(VideoClip clip, System.Action then = null)
    {
        Audio.PlaySFX(SFX_IncomingVideo);
        UDP.Write(GAME.UDP_ReduceMusicVolume);
        this.ActionAfterSecondDelay(videoDelayWithAnnouncementSound, () =>
        {
            videoPlayer_TV.PlayVideo(clip, false, null, false, ()=> { UDP.Write(GAME.UDP_IncreaseMusicVolume); if (then != null) { then(); } });
        });
    }

    public void ResetGameButton()
    {
        if (GAME.gameHasEnded)
        {
            ResetGame();
        }
    }

    void ResetGame()
    {
        UDP.Write(GAME.UDP_GameReset);
        GAME.ResetGame();
    }

    void LoseGame()
    {
        if (GAME.gameHasStarted && !GAME.gameHasEnded)
        {
            UDP.Write(GAME.UDP_GameLost);
            GAME.gameHasEnded = true;
            PlayVideo(loseVideo, () =>
            {
                this.ActionAfterFrameDelay(1, () =>
                {
                    videoPlayer_TV.PlayVideo(idle_LobbyTV_Video, true);
                    if (loseScreens != null)
                    {
                        for (int i = 0; i < loseScreens.Length; loseScreens[i++].SetActive(true)) ;
                    }
                });
            });
        }
    }

    void OnLastItemScannedBeforePuzzleCompleted(Item lastItem)
    {
        TV.SetLastItemIncorrectOrderScreenVisibleState(true, lastItem);
    }


    public void RFID_Scan(string ID)
    {
        UDP.Write("RFID_" + ID);
        Item item = ItemDatabase.GetItem(ID);
        if (item != null && item.isKey)
        {
            lastItemScanned = true;
            if (TV.allItemsScanned)
            {
                GameCompleted();
            }
            else
            {
                OnLastItemScannedBeforePuzzleCompleted(item);
            }
        }
    }
}
