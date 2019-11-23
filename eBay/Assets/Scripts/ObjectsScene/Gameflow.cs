using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gameflow : MonoBehaviour
{
    [Header("Settings")]
    public bool usingDebugCanvas = false;
    public int maxObjectsCount = 12;

    [Header("UDP")]
    public string UDP_ToReceive_RFID = "RFID_";
    public string UDP_ToReceive_SetMaxObjectCount = "SETUP_OBJECTCOUNT_";

    [Header("Components")]
    public GameObject[] winScreens;
    public GameObject[] loseScreens;
    public Canvas debugCanvas;
    public Canvas[] canvases;
    public ItemGrid[] itemGrids;
    public ItemGrid[] debugItemGrids;

    [Header("Audio")]
    public AudioClip MUSIC_15MIN;
    public AudioClip MUSIC_10MIN;

    [Header("Videos")]
    public VideoPlayer[] videoPlayers_idles;
    public UnityEngine.Video.VideoClip idleVideo;

    Item[] Items;

    private void Start()
    {
        Audio.DestroyAllSounds();
        Items = ItemDatabase.AllItems.Where((item) => { return item.isOnSilluetePuzzle; }).ToArray();
        Debug.Log(Items.Length);
        UDP.onMessageReceived = UDP_COMMAND;
        debugCanvas.gameObject.SetActive(usingDebugCanvas);
        if (canvases != null && canvases.Length > 0 && canvases[0] != null) { canvases[0].gameObject.SetActive(!usingDebugCanvas); }
        SetIdelVideoVisibleState(true);
        Setup();
    }

    public void SetIdelVideoVisibleState(bool isVisible)
    {
        if (videoPlayers_idles != null && videoPlayers_idles.Length > 0)
        {
            for (int i = 0; i < videoPlayers_idles.Length; i++)
            {
                if (isVisible && !videoPlayers_idles[i].IsPlaying) { videoPlayers_idles[i].PlayVideo(idleVideo, true, null); }
                else if (!isVisible) { videoPlayers_idles[i].Close(); }
            }
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

        ItemGrid[] grids = usingDebugCanvas ? debugItemGrids : itemGrids;

        if (grids != null)
        {
            int totalAmountOfItemsToUse = itemCount < 0 ? ItemDatabase.Count : Mathf.Clamp(itemCount, 0, ItemDatabase.Count);
            float _itemsPerGrid = totalAmountOfItemsToUse / (float)grids.Length; // evenDistribution;
            int itemsPerGrid = Mathf.FloorToInt(_itemsPerGrid);
            int remaining = Mathf.RoundToInt((_itemsPerGrid - itemsPerGrid) * grids.Length);

            for (int i = 0; i < grids.Length; i++)
            {
                Item.Category category = (Item.Category)i;
                Item[] possibleItems = Items.Where((item) => { return item.category == category; }).ToArray();
                int itemCountForThisGrid = itemsPerGrid + (remaining-- > 0 ? 1 : 0);
                int nextPackageSize = Mathf.Clamp(itemCountForThisGrid, 0, grids[i].capacity);
                nextPackageSize = Mathf.Min(nextPackageSize, possibleItems == null ? 0 : possibleItems.Length);


                Item[] nextItemPack = new Item[nextPackageSize];

                for (int x = 0; x < nextItemPack.Length; x++)
                {
                    nextItemPack[x] = possibleItems[x];
                }

                grids[i].Setup(nextItemPack, category);

            }
        }
    }

    void UDP_COMMAND(string command)
    {
        if (!string.IsNullOrEmpty(command))
        {
            if (command.Length > UDP_ToReceive_RFID.Length && command.ToLower().Substring(0, UDP_ToReceive_RFID.Length) == UDP_ToReceive_RFID.ToLower())
            {
                string ID = command.Substring(UDP_ToReceive_RFID.Length);
                TAG_SCANNER(ID);
            }

            if (command.Length > UDP_ToReceive_SetMaxObjectCount.Length && command.ToLower().Substring(0, UDP_ToReceive_SetMaxObjectCount.Length) == UDP_ToReceive_SetMaxObjectCount.ToLower())
            {
                string valueSTR = command.Substring(UDP_ToReceive_SetMaxObjectCount.Length);
                int value = maxObjectsCount;
                if (int.TryParse(valueSTR, out value))
                {
                    value = Mathf.Clamp(value, 0, ItemDatabase.Count);
                    Setup(value);
                }
            }

            if (command.ToLower() == GAME.UDP_GameCompleted.ToLower()) { GameCompleted(); }
            if (command.ToLower() == GAME.UDP_GameLost.ToLower()) { LoseGame(); }
            if (command.ToLower() == GAME.UDP_GameStart.ToLower()) { StartGame(); }
            if (command.ToLower() == GAME.UDP_GameReset.ToLower()) { GAME.ResetGame(); }
            if (command.Length > GAME.UDP_SetGameTimer.Length && command.ToLower().Substring(0, GAME.UDP_SetGameTimer.Length) == GAME.UDP_SetGameTimer.ToLower())
            {
                string valueSTR = command.Substring(GAME.UDP_SetGameTimer.Length);
                int value = (int)GAME.duration;
                if (int.TryParse(valueSTR, out value)) { GAME.duration = value; }
            }
        }
    }

    void StartGame()
    {
        GAME.StartGame();
        SetIdelVideoVisibleState(false);
        Audio.PlayMusic(GAME.duration < 900 ? MUSIC_10MIN : MUSIC_15MIN);
    }

    public void GameCompleted()
    {
        if (!GAME.gameHasEnded)
        {
            GAME.gameHasEnded = true;
            if (winScreens != null)
            {
                for (int i = 0; i < winScreens.Length; winScreens[i++].SetActive(true)) ;
            }
            if (Audio.MusicChannel != null) { Audio.MusicChannel.Stop(1); }
        }
    }
    void LoseGame()
    {
        if (GAME.gameHasStarted && !GAME.gameHasEnded)
        {
            GAME.gameHasEnded = true;
            if (loseScreens != null)
            {
                for (int i = 0; i < loseScreens.Length; loseScreens[i++].SetActive(true)) ;
            }
            if (Audio.MusicChannel != null) { Audio.MusicChannel.Stop(1); }
        }
    }


    void TAG_SCANNER(string scan)
    {
        ItemGrid[] grids = usingDebugCanvas ? debugItemGrids : itemGrids;
        if (grids != null)
        {
            bool found = false;
            for (int i = 0; i < grids.Length; i++)
            {
                found = grids[i].Reveal(scan);
                if (found) { return; }
            }
        }
    }

}
