using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OVR.API;
public class ConnectionUI : MonoBehaviour
{
    [SerializeField] Image bt;
    [SerializeField] Image wifi;
    [SerializeField] Image usb;

    [SerializeField] Color def;
    [SerializeField] Color connected;
    [SerializeField] Color error;

    [SerializeField] Button autoConnectButton;
    [SerializeField] Button disconnectButton;

    DeviceState currentState;



    // Start is called before the first frame update
    void Start()
    {
        SetDefault();
        disconnectButton.interactable = false;
        //autoConnectButton.interactable = true;
    }
    void Update()
    {
        if (ConnectionManager.instance.isConnected && currentState == DeviceState.NO_STATE)
        {
            currentState = ConnectionManager.instance.deviceState;
            Connected();
        }
        else if (!ConnectionManager.instance.isConnected && currentState != DeviceState.NO_STATE)
        {
            ConnectedError();
        }
    }

    public void Connected()
    {
        ThinkingBar.instance.Hide();
        ConnectionSettings.instance.UpdateConnectionName(ConnectionManager.instance.activeDeviceName);
        disconnectButton.interactable = true;
        //autoConnectButton.interactable = false;
        SetDefault();
        switch (currentState) {
            case DeviceState.BLE_STATE:
                bt.color = connected;
                break;
            case DeviceState.SERIAL_STATE:
                usb.color = connected;
                break;
        }
    }
    public void ConnectedError()
    {
        ConnectionSettings.instance.UpdateConnectionName("...");
        disconnectButton.interactable = false;
        //autoConnectButton.interactable = true;
        SetDefault();
        switch (currentState)
        {
            case DeviceState.BLE_STATE:
                bt.color = error;
                break;
            case DeviceState.SERIAL_STATE:
                usb.color = error;
                break;
        }
        currentState = DeviceState.NO_STATE;
        //wait 2 seconds and reset
        Invoke("SetDefault", 0.75f);
    }

    public void SetDefault()
    {
        if (bt != null)
        {
            bt.color = def;
            wifi.color = def;
            usb.color = def;
        }
    }

}
