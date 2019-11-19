using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Right,
    Left,
    Bottom,
    Top
}

public class Screens : MonoBehaviour {

    public delegate void CALLBACK();
    public CALLBACK OnScreenOpens = delegate () { };
    public CALLBACK OnScreenCloses = delegate () { };
    public CALLBACK OnPurposeComplete = delegate () { };

    public GameObject screen;
    public bool isOpen { private set { _isOpen = value; } get { return this.screen.activeInHierarchy; } }
    bool _isOpen;

    protected virtual void Awake() {
        Close();
    }

    public virtual void Close() {
        isOpen = false;
        screen.SetActive(false);
        OnScreenCloses();
    }

    public virtual void CloseImmediate() {
        isOpen = false;
        screen.SetActive(false);
        OnScreenCloses();
    }

    

    public virtual void Open() {
        isOpen = true;
        screen.SetActive(true);
        OnScreenOpens();
    }

    public virtual void Complete() {
        OnPurposeComplete();
    }
    public virtual void CompleteAfterTime(float waitTime) {
        this.ActionAfterSecondDelay(waitTime, Complete);
    }

    public virtual void EnterFromDirection(Direction dir, float duration) {
        Vector3 screenOffsetStart = Vector3.zero;
        if (dir == Direction.Left || dir == Direction.Right) {
            screenOffsetStart += Vector3.right * (dir == Direction.Right ? 1f : -1f) * Screen.width;
        } else if (dir == Direction.Top || dir == Direction.Bottom) {
            screenOffsetStart += Vector3.up * (dir == Direction.Top ? 1f : -1f) * Screen.height;
        }
        transform.localPosition = screenOffsetStart;
        float startTime = Time.time;
        this.InterpolateCoroutine(duration, (x) => { transform.localPosition = Vector3.Lerp(screenOffsetStart, Vector3.zero, (Time.time - startTime) / duration); });
    }

    public virtual void LeaveToDirection(Direction dir, float duration) {
        Vector3 screenOffsetEnd = Vector3.zero;
        if (dir == Direction.Left || dir == Direction.Right) {
            screenOffsetEnd += Vector3.right * (dir == Direction.Right ? 1f : -1f) * Screen.width;
        } else if (dir == Direction.Top || dir == Direction.Bottom) {
            screenOffsetEnd += Vector3.up * (dir == Direction.Top ? 1f : -1f) * Screen.height;
        }
        
        float startTime = Time.time;
        this.InterpolateCoroutine(duration, (x) => { transform.localPosition = Vector3.Lerp(Vector3.zero, screenOffsetEnd, (Time.time - startTime) / duration); });
    }


    public virtual void UDP_COMMANDS(string command) {

    }

    public virtual void KeyboardCommands() {

    }
}
