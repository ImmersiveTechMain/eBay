using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScreen : MonoBehaviour
{
    public Color digitsColor; 
    public Text minutes_Tens;
    public Text minutesUnits;
    public Text seconds_Tens;
    public Text secondsUnits;
    
    // Update is called once per frame
    void Update()
    {
        float time = GAME.timer == null ? GAME.duration : GAME.timer.timeRemaining;
        string parsedValue = SkeetoTools.Timer.FormatToDisplay(time, false, false, true);

        if (minutes_Tens.text != parsedValue[0].ToString()) { minutes_Tens.StopAllCoroutines(); minutes_Tens.InterpolateCoroutine(0.3f, (n) => { minutes_Tens.color = Color.Lerp(Color.white, digitsColor, n);  }); }
        minutes_Tens.text = parsedValue[0].ToString();
        if (minutesUnits.text != parsedValue[1].ToString()) { minutesUnits.StopAllCoroutines(); minutesUnits.InterpolateCoroutine(0.3f, (n) => { minutesUnits.color = Color.Lerp(Color.white, digitsColor, n);  }); }
        minutesUnits.text = parsedValue[1].ToString();
        if (seconds_Tens.text != parsedValue[3].ToString()) { seconds_Tens.StopAllCoroutines(); seconds_Tens.InterpolateCoroutine(0.3f, (n) => { seconds_Tens.color = Color.Lerp(Color.white, digitsColor, n);  }); }
        seconds_Tens.text = parsedValue[3].ToString();
        if (secondsUnits.text != parsedValue[4].ToString()) { secondsUnits.StopAllCoroutines(); secondsUnits.InterpolateCoroutine(0.3f, (n) => { secondsUnits.color = Color.Lerp(Color.white, digitsColor, n);  }); }
        secondsUnits.text = parsedValue[4].ToString();
    }

}
