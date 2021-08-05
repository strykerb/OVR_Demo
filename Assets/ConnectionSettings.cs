using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OVR.API;
//using ArduinoBluetoothAPI;

public class ConnectionSettings : MonoBehaviour
{
    [SerializeField] Button bleButtonPrefab;
    [SerializeField] Button serialButtonPrefab;
    [SerializeField] Transform deviceHolder;
    [SerializeField] GridLayoutGroup glg;
    [SerializeField] TextMeshProUGUI connectionNameTF;
    [SerializeField] GameObject autoX;


    GameObject content;

    public static ConnectionSettings instance;
    private void Awake(){
        instance = this;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        content = transform.Find("Content").gameObject;
        content.SetActive(false);

    }

    void Update()
    {
        CheckInput();
    }
    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            HideShow();
        }
    }

    void HideShow()
    {
        if (content.activeInHierarchy)
            content.SetActive(false);
        else
        {
            content.SetActive(true);
            Init();
        }
    }
    void Init()
    {
        if (ConnectionManager.instance.isAutoConnect)
        {
            autoX.SetActive(true);
        }
        else
        {
            autoX.SetActive(false);
        }
    }

    //Manually Connect
    public void FindBleDevices()
    {
        CleanButtons();
        ConnectionManager.instance.Init(OVR.API.DeviceState.BLE_STATE);
        ConnectionManager.instance.StartScan(false);
        autoX.SetActive(false);
    }
    public void FindSerialDevices()
    {
        CleanButtons();
        ConnectionManager.instance.isAutoConnect = false;
        ConnectionManager.instance.Init(OVR.API.DeviceState.SERIAL_STATE);
        autoX.SetActive(false);
    }

    void SetGridSize(int count)
    {
        
        Vector2 cellSize = Vector2.zero;
        switch (count)
        {
            case 1:
                cellSize = new Vector2(470, 135);
                break;
            case 2:
                cellSize = new Vector2(232.5f, 135);
                break;
            case 3:
            case 4:
                cellSize = new Vector2(232.5f, 65);
                break;
            case 5:
            case 6:
                cellSize = new Vector2(232.5f, 42);
                break;
            case 7:
            case 8:
                cellSize = new Vector2(232.5f, 30);
                break;
        }
        glg.cellSize = cellSize;
    }
    public void BuildBleButtons(List<string> names)
    {
        Debug.Log(OVRHelper.OVR + "BuildBleButtons()");
        ThinkingBar.instance.Hide();
        CleanButtons();
        //set of the grid
        SetGridSize(names.Count);
        foreach(string name in names)
        {
            Button button = Instantiate(bleButtonPrefab, deviceHolder);
            button.GetComponent<BleButton>().Init(name);
        }
    }
    public void BuildSerialButtons(List<string> names)
    {
        Debug.Log(OVRHelper.OVR + "BuildSerialButtons()");
        if(ThinkingBar.instance)
            ThinkingBar.instance.Hide();
        CleanButtons();
        //set of the grid
        SetGridSize(names.Count);
        foreach (string name in names)
        {
            Button button = Instantiate(serialButtonPrefab, deviceHolder);
            button.GetComponent<SerialButton>().Init(name);
        }
    }
    public void CleanButtons()
    {
        for (int i = 0; i < deviceHolder.transform.childCount; i++)
        {
            Destroy(deviceHolder.transform.GetChild(i).gameObject);
        }
    }

    public void UpdateConnectionName(string str)
    {
        connectionNameTF.text = str;
    }
    public void HandleDisconnectButton()
    {
        ConnectionManager.instance.Dissconnect();
    }
    public void HandAutoConnectButton()
    {
        Debug.Log("HandAutoConnectButton()");
        Debug.Log(ConnectionManager.instance.isAutoConnect);
        if (ConnectionManager.instance.isAutoConnect)
        {
            //turn off auto Connect
            ConnectionManager.instance.isAutoConnect = false;
            //need to stop other stuff probably
            autoX.SetActive(false);
        }
        else
        {
            autoX.SetActive(true);
            ConnectionManager.instance.isAutoConnect = true;
            AutoConnect.instance.Init();
        }
        
    }

    public void NoDevicesFound()
    {
        ThinkingBar.instance.UpdateText("No Devices Found...");
        if (!ConnectionManager.instance.isAutoConnect)
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(1f);
                ThinkingBar.instance.Hide();
            }
        }
    }
    public void UnableToConnect()
    {
        ThinkingBar.instance.UpdateText("Unable to connect:(");
        if (!ConnectionManager.instance.isAutoConnect)
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(1f);
                ThinkingBar.instance.Hide();
            }
        }
    }

}
