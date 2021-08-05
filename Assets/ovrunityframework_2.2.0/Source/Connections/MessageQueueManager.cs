using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OVR.API;

public class MessageQueueManager:MonoBehaviour
{
    static float timeOutMax = 0.4f;
    static float timeOut;
    static float timeOutCount = 0;
    public struct LastMessage
    {
        public byte[] data;
        public float timeStamp;
        public int responseInt;
    }
    [SerializeField] public static Queue<byte[]> messageQueue;
    public static LastMessage lastMessage = new LastMessage();

    public static void ResetMessageQueue()
    {
        if(messageQueue!=null)
            messageQueue.Clear();
    }

    public static void Add(byte[] message)
    {
        Debug.Log(OVRHelper.OVR + "Add message to MessageQueManager");
        if (messageQueue == null)
        {
            messageQueue = new Queue<byte[]>();
        }

        //fix for set state at beginning and end of connection
        if ((int)message[1] == 16)
        {
            ConnectionManager.instance.Send(message);
            return;
        }
        messageQueue.Enqueue(message);
    }


    private void Update()
    {
        if (messageQueue != null)
        {

            CheckForMessages();
        }  
    }
    void CheckForMessages()
    {
        if (lastMessage.timeStamp == 0)
        {
            if (messageQueue.Count > 0)
            {
                byte[] packet = messageQueue.Dequeue();
                timeOut = Time.time;
                /*
                //check for ending message
                if ((int)packet[0] == 100)
                {
                    timeOutCount = 0;
                    //0 = init
                    //1 = MFG General
                    //2 = set defaults
                    switch((int)packet[1]){
                        case 0:
                            lastMessage.timeStamp = 0;
                            DeviceStatus.instance.FinishRefresh();
                            AdminDebug.Print("END INIT", PrintState.RESPONSE);
                            break;
                        case 1:
                            lastMessage.timeStamp = 0;
                            MfgGeneral.instance.LoadComplete();
                            AdminDebug.Print("END MFG Genreal", PrintState.RESPONSE);
                            break;
                        case 2:
                            lastMessage.timeStamp = 0;
                            MfgScentNames.instance.LoadComplete();
                            AdminDebug.Print("END MFG Scent Names", PrintState.RESPONSE);
                            break;
                        case 3:
                            lastMessage.timeStamp = 0;
                            MfgFrequency.instance.LoadComplete();
                            AdminDebug.Print("END MFG Scent Names", PrintState.RESPONSE);
                            break;
                        case 4:
                            lastMessage.timeStamp = 0;
                            MfgMaxT.instance.LoadComplete();
                            AdminDebug.Print("END MFG TMaxT", PrintState.RESPONSE);
                            break;
                        case 5:
                            lastMessage.timeStamp = 0;
                            MfgGeneral.instance.OnSubmit();
                            AdminDebug.Print("END MFG General Submit", PrintState.RESPONSE);
                            break;
                        case 6:
                            lastMessage.timeStamp = 0;
                            MfgScentNames.instance.OnSubmit();
                            AdminDebug.Print("END MFG Scent Submit", PrintState.RESPONSE);
                            break;
                        case 7:
                            lastMessage.timeStamp = 0;
                        
                            MfgFrequency.instance.OnSubmit();
                            AdminDebug.Print("END MFG Frequency Submit", PrintState.RESPONSE);
                            break;
                        case 8:
                            lastMessage.timeStamp = 0;
                            MfgMaxT.instance.OnSubmit();
                            AdminDebug.Print("END MFG TMaxT Submit", PrintState.RESPONSE);
                            break;
                    }
                }
                else
                {*/
                    //AdminDebug.Print(Enum.GetName(typeof(MessageTypes), packet[1]) + " : " + ByteHelper.CovertToReadableByteString(packet), PrintState.REQUEST);
                    ConnectionManager.instance.Send(packet);
                    SetResponse(packet);
                    
                //}

            }
        }
        else if(Time.time-timeOut > timeOutMax)
        {
            timeOutCount++;
            //for testing serial
            if (lastMessage.responseInt == 7 && ConnectionManager.instance.serialTestState == "new")
            {
                ConnectionManager.instance.serialTestState = "failure";
                timeOutCount = 0;
                messageQueue.Clear();
            }


            AdminDebug.Print("TIME OUT", PrintState.ERROR);
            if (timeOutCount == 3)
            {
                //IntroAnimation.instance.ShowBigSurBug();
                timeOutCount = 0;
                messageQueue.Clear();
                //DeviceStatus.instance.StopInitCoroutine();
                //DeviceStatus.instance.FinishRefreshError();
                //AdminDebug.Print("Init ERROR", PrintState.ERROR);
                
            }
            lastMessage.timeStamp = 0;
        }
    }
    void SetResponse(byte[] packet)
    {
        int messageType = (int)packet[1];
        if (messageType == 16 || messageType == 19 || messageType == 20 || (messageType > 34 && messageType < 48))
            return;
        if (messageType == 27) messageType--;
        else messageType++;
        lastMessage.data = packet;
        lastMessage.timeStamp = Time.time;
        lastMessage.responseInt = messageType;
    }
    public static void HandleResponse(byte[] packet)
    {
        if ((int)packet[1] == lastMessage.responseInt)
        {
            float elapsedTime = Time.time - lastMessage.timeStamp;
            lastMessage.timeStamp = 0;

            AdminDebug.Print(Enum.GetName(typeof(MessageTypes), packet[1]) + ": " + ByteHelper.CovertToReadableByteString(packet), PrintState.RESPONSE);
            AdminDebug.Print(elapsedTime.ToString(), PrintState.TIME);

        }
        else
        {
            //lastMessage.timeStamp = 0;
            AdminDebug.Print("MessageQue, not a response to previois message:"+ ByteHelper.CovertToReadableByteString(packet), PrintState.ERROR);
        }

    }

}
