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

    [Header("Components")]
    public GameObject[] winScreens;
    public GameObject[] loseScreens;
    public VideoPlayer videoPlayer_TV;
    [Header("Gameflows")]
    public Gameflow_TV TV;
    public Gameflow_Selfcheckout Selfcheckout;

    [Header("Videos")]
    public IntroVideo[] introVideos;
    public VideoClip allScannedVideo;
    public VideoClip winVideo;
    public VideoClip loseVideo;

    [System.Serializable]
    public struct IntroVideo
    {
        public SettingsScreen.ItemCountOptions itemCount;
        public VideoClip video;
    }

    public VideoClip GetIntroVideo()
    {
        if (introVideos == null || introVideos.Length <= 0) { return null; }
        for (int i = 0; i < introVideos.Length; i++)
        {
            if (introVideos[i].itemCount == SettingsScreen.itemCountOptions) { return introVideos[i].video; }
        }
        return null;
    } 
    
    bool lastItemScanned = false;

    // Start is called before the first frame update
    void Start()
    {
        SettingsScreen.ApplyUserSettings();
        Setup();
        TV.OnAllItemsChecked = () => { if (lastItemScanned) { GameCompleted(); } else { videoPlayer_TV.PlayVideo(allScannedVideo); } };
        SERIAL.onMessageReceived = RFID_Scan;
        SERIAL.onMessageReceived += TV.RFID_Scan;
        SERIAL.onMessageReceived += Selfcheckout.RFID_Scan;
        TV.usingPasscodeNumbers = usingPasscodeNumbers;
    }

    public void StartGame()
    {
        if (!GAME.gameHasStarted)
        {
            lastItemScanned = false;
            Setup(SettingsScreen.GetSettings_ItemCount());
            videoPlayer_TV.PlayVideo(GetIntroVideo());
            GAME.StartGame();
            GAME.timer.OnTimerEnds = LoseGame;
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

        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift)) { ResetGame(); }
    }

    public void GameCompleted()
    {
        if (!GAME.gameHasEnded)
        {
            GAME.gameHasEnded = true;
            GAME.timer.Pause();
            TV.SetLastItemIncorrectOrderScreenVisibleState(false, null);
            videoPlayer_TV.PlayVideo(winVideo, false, null, false, () =>
            {
                UDP.Write(GAME.UDP_GameCompleted);
                if (winScreens != null)
                {
                    for (int i = 0; i < winScreens.Length; winScreens[i++].SetActive(true)) ;
                }
            });
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
            videoPlayer_TV.PlayVideo(loseVideo, false, null, false, ()=> 
            {
                if (loseScreens != null)
                {
                    for (int i = 0; i < loseScreens.Length; loseScreens[i++].SetActive(true)) ;
                }
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
