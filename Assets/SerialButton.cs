using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SerialButton : MonoBehaviour
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
            //check to see if we are already connected to same one
            if (ConnectionManager.instance.activeDeviceName == deviceName)
            {
                Debug.Log("Already Connected...");
                ThinkingBar.instance.Show("Already Connected");
                Invoke("HideThinkingBar", 1f);
                return;
            }
            else
            {

                Debug.Log("Serial Button: Disconnect then connect");
                ConnectionManager.instance.Dissconnect(deviceName);
                ConnectionSettings.instance.CleanButtons();
                //Invoke("Connect", 1f);
                return;
            }
        }
        Connect();   
    }

    void HideThinkingBar()
    {
        ThinkingBar.instance.Hide();
    }
    void Connect()
    {
        Debug.Log("Serial Button: Connect");

        ConnectionManager.instance.CheckSingleSerial(deviceName);
        ConnectionSettings.instance.CleanButtons();
    }
}
