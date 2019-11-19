using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // go to unity settings and change the .Net version to enable this library

public class SERIAL : MonoBehaviour
{
    public delegate void CALLBACK(string msg);
    public static CALLBACK onMessageReceived = delegate (string msg) { };

    [Header("Settings")]
    public string PortName;
    public byte slaveAddress = 0;
    public float checkForAnswerRate = 0.45f;
    public int readTimeout = 1;
    public int writeTimeout = 10;
    public int baudRate_BitPerSeconds = 9600;
    public float writeCooldown = 2;
    public string prefixIdentifier = "0A"; // Optional, will ignore all messages that dont beging with this prefix
    public string sufixIdentifier = "0D"; // Optional, will ignore all messages that dont end with this sufix

    static SerialPort serialPort;
    static bool initialized = false;

    public Queue<string> writteQueue = new Queue<string>();

    public object DebugPorts { get; private set; }
    static bool isListening = false;

    private void Start()
    {
        if (!Initialize()) { return; }

        if (!string.IsNullOrEmpty(PortName))
        {
            CreatePortObject(); // 1. Create the port object
            OpenPort(); // 2. Open port
            Listen(); // 3. Start Listening throgh it
        }
        else
        {
            CheckForPortExistence("Example");
        }
    }

    bool Initialize()
    {
        if (initialized) { Destroy(gameObject); return false; } // 1. Stop the script if another exists
        initialized = true;
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        return true;
    }

    void CreatePortObject()
    {
        // Parity.Even is for debugging, as I understand it is checking that all the data you are receiving correspond to the same index than the one you send? may be
        // the number 8 is data bits ( the number of bits that represents one character of data)
        serialPort = new SerialPort(PortName, baudRate_BitPerSeconds, Parity.None, 8, StopBits.One);
        serialPort.Handshake = Handshake.None;
    }

    bool OpenPort()
    {
        if (serialPort != null)
        {
            if (serialPort.IsOpen) { Debug.Log("The port was already open! "); return true; }
            else
            {
                //serialPort.RtsEnable = true;
                serialPort.DtrEnable = true;
                serialPort.ReadTimeout = readTimeout;
                serialPort.WriteTimeout = writeTimeout;
                serialPort.Open();
                //serialPort.DataReceived += OnDataReceived;
                serialPort.ErrorReceived += SerialPort_ErrorReceived;
                serialPort.BaseStream.Flush(); // So as i understood, flush means to use / write the data you have been holding in a temporal buffer                
                Debug.Log("Port " + PortName + " is now Open.");
                return true;
            }
        }
        return false;
    }

    bool ClosePort()
    {
        if (serialPort != null && serialPort.IsOpen) { serialPort.Close(); return true; }
        return false;
    }

    bool CheckForPortExistence(string portName)
    {
        Debug.Log("Getting all available ports...");
        string[] portsAvailable = SerialPort.GetPortNames();

        if (portsAvailable == null) { Debug.Log("No ports are available"); return false; }

        for (int i = 0; i < portsAvailable.Length; i++)
        {
            Debug.Log("Port [" + (i + 1) + "]: " + portsAvailable[i]);
            if (portsAvailable[i] == portName) { return true; }
        }

        return false;
    }

    public void Listen()
    {
        if (!isListening) { StartCoroutine(_Listen()); }
    }

    IEnumerator _Listen()
    {
        float lastMessageSentTime = 0;
        isListening = true;

        while (serialPort != null && serialPort.IsOpen && isListening)
        {
            // WRITE ---------------------------------------------------------------------------------
            if (writteQueue.Count > 0 && Time.time - lastMessageSentTime > writeCooldown)
            {
                string messageToSend = writteQueue.Dequeue();
                try
                {
                    serialPort.WriteLine(messageToSend);
                    Debug.Log("Writing " + messageToSend);
                    lastMessageSentTime = Time.time;
                }
                catch (System.Exception e) { Debug.Log(e.Message); }
                yield return new WaitForSecondsRealtime(checkForAnswerRate);
            }

            // READ ----------------------------------------------------------------------------------
            try
            {
                byte[] byteAnswer = new byte[12];
                serialPort.Read(byteAnswer, 0, byteAnswer.Length);
                OnBytesRecieved(byteAnswer, true);
            }
            catch (System.Exception e) { }

            yield return new WaitForSecondsRealtime(checkForAnswerRate);
        }

        isListening = false;
    }

    public string OnBytesRecieved(byte[] answer, bool fromMainPort)
    {
        string message = "";
        string parsed = "";
        string messageAsID = "";

        for (int i = 0; i < answer.Length; i++)
        {
            message += answer[i].ToString();
            message += " ";

            parsed += string.Format("{0:X2}", answer[i]);
            messageAsID += string.Format("{0:X2}", answer[i]);
            parsed += " ";
        }

        string asciiParsedCode = System.Text.Encoding.ASCII.GetString(answer);
        asciiParsedCode = asciiParsedCode.Trim('\0');

       // Debug.Log("Intact Message received = " + message + "\nASCII Code:" + asciiParsedCode);
        //Debug.Log("Pure: " + message + "\n Parsed: " + parsed);

        if (!string.IsNullOrEmpty(prefixIdentifier) && prefixIdentifier.Length < messageAsID.Length && messageAsID.Substring(0,prefixIdentifier.Length) != prefixIdentifier) { return null; }
        if (!string.IsNullOrEmpty(sufixIdentifier) && sufixIdentifier.Length < messageAsID.Length && messageAsID.Substring(messageAsID.Length - sufixIdentifier.Length) != sufixIdentifier) { return null; }


        Debug.Log("ID: " + messageAsID);

        onMessageReceived(messageAsID);
        return messageAsID;
    }
    public void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        serialPort.DiscardInBuffer();
        serialPort.DiscardOutBuffer();
    }

    private void OnApplicationQuit()
    {
        isListening = false;
        ClosePort();
    }
}
