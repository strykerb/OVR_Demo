using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;

class MyStreamManager : ArduinoBluetoothAPI.StreamManager
{
    public override byte[] formatDataToSend(byte[] buff)
    {
        return buff;
    }

    public override void handleReceivedData(byte[] buff)
    {
        this.OnDataReceived.Invoke(buff); //Invoke the OnDataReceived method
    }
}