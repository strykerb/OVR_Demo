using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BleButton : MonoBehaviour
{
    string deviceName;
    [SerializeField] TextMeshProUGUI tf;
    public void Init(string str)
    {
        deviceName = str;
        tf.text = str;
    }

    public void OnButtonClick()
    {
        Debug.Log("OnButtonClick()");
        
        if (ConnectionManager.instance.isConnected)
        {
            ConnectionManager.instance.Dissconnect();
            Invoke("Connect", 1f);
            return;
        }
        Connect();   
    }

    void Connect()
    {
        Debug.Log("BLE Button: Connect");

        ConnectionManager.instance.Connect(deviceName);
        ConnectionSettings.instance.CleanButtons();
    }
}
