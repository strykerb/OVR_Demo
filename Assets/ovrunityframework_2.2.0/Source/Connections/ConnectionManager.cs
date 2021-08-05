using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.UI;
using TMPro;
using OVR.API;

public class ConnectionManager : MonoBehaviour
{
    //General
    public bool isAutoConnect;
    public static ConnectionManager instance;
    public string connectionType;
    public DeviceState deviceState;
    public bool isConnected;
    public string activeDeviceName = "";
    private int scanCounter = 0;
    private int scanCounterMax = 1;//how many time to search scan before giving up

    public string serialTestState = "";
    int serialTestCounter = 0;
    List<string> portNamesToTest = new List<string>();
    

    //BLE
    private LinkedList<BluetoothDevice> devices;
    private BluetoothHelper helper;
    [SerializeField] List<GameObject> connectedBLEDevices = new List<GameObject>();
    public bool bleFirstTime = true;

    //UBS
    public SerialControllerOVR serialController;

    private void Awake(){instance = this;}
    
    private void OnEnable()
    {
        //add the serial controller
        serialController = gameObject.AddComponent<SerialControllerOVR>();
        gameObject.AddComponent<OdorantManager>();
        gameObject.AddComponent<MessageQueueManager>();
    }
    private void OnDisable()
    {
        //BLE, need to add for serial?
        if (helper != null)
        {
            helper.Disconnect();
        }
        isConnected = false;
        connectionType = "";
    }

    public void Init(DeviceState ds) {
        Debug.Log(OVRHelper.OVR + "ConnectionManager: Init(): " + ds);
        deviceState = ds;
        switch (deviceState)
        {
            case DeviceState.BLE_STATE:
                if (bleFirstTime)
                {
                    try
                    {
                        BluetoothHelper.BLE = true;
                        helper = BluetoothHelper.GetInstance();
                        helper.OnConnected += OnConnected;
                        helper.OnConnectionFailed += OnConnectionFailed;
                        helper.OnScanEnded += OnScanEnded;
                        helper.OnDataReceived += OnDataReceived;
                        helper.setCustomStreamManager(new MyStreamManager());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    bleFirstTime = false;
                }
                break;
            case DeviceState.SERIAL_STATE:
                GetSerialPortNames();
                break;
            case DeviceState.OTG_State:
                break;
        }      
    }
    public void GetSerialPortNames()
    {
        string[] portNames = SerialControllerOVR.GetPortNames();
        List<string> validPortNames = new List<string>();

        foreach (var portName in portNames)
        {
            if ((portName.Contains("UART") || portName.Contains("uart") || portName.Contains("serial") || portName.Contains("COM") || portName.Contains("com")))// && !portName.Contains("coming"))
            {
                Debug.Log(OVRHelper.OVR + "port name: " + portName);
                validPortNames.Add(portName);
            }
        }
        if (validPortNames.Count == 0)
        {
            Debug.Log(OVRHelper.OVR + "NO Serial ports found!");
            AutoConnect.instance.HandleNoConnectionAvailable();
        }

        else if (validPortNames.Count == 1)
        {
            Debug.Log(OVRHelper.OVR + "1 Serial Port Found!");
            if (isAutoConnect)
                CheckMultipleSerial(validPortNames);
            else
                ConnectionSettings.instance.BuildSerialButtons(validPortNames);
        }
        else if (validPortNames.Count > 1)
        {
            //multiple devices
            Debug.Log(OVRHelper.OVR + "Multiple Ports found!");
            if (isAutoConnect)
                CheckMultipleSerial(validPortNames);
            else
                ConnectionSettings.instance.BuildSerialButtons(validPortNames);
        }
    }

    public void CheckSingleSerial(string portName)
    {
        Debug.Log(OVRHelper.OVR + "CheckSingleSerial()");
        if (Connect(portName))
        {
            serialTestState = "new";
            TestConnection(portName);
        }
        else
        {
            Debug.Log(OVRHelper.OVR + "Cannot connect to Serial Port.");
            Dissconnect();
            //DisconnectSerial();
        }
    }
    public void LoopThroughSerial(int i)
    {
        CheckSingleSerial(portNamesToTest[i]);
        serialTestCounter = i + 1;
    }

    public void CheckMultipleSerial(List<string> ports) {
        portNamesToTest = ports;
        LoopThroughSerial(0);
    }

    public void TestConnection(string portName)
    {
        Debug.Log(OVRHelper.OVR + "TestConnection: " + portName);
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.5f);
            //send set serial state message
            SetDeviceState();
            yield return new WaitForSeconds(0.5f);
            //wait
            //send test message DEVICE_NAME_REQUEST
            SendData(MessageTypes.DEVICE_NAME_REQUEST);
            yield return new WaitForSeconds(0.5f);
            if (serialTestState == "success")
            {
                activeDeviceName = portName;
                InitConnected();
            }
            else
            {
                Debug.Log(OVRHelper.OVR + "Valid ION V2 Test failed.");
                Dissconnect();
                //DisconnectSerial();
            }

        }
        
    }

    public bool Connect(string str)
    {
        Debug.Log(OVRHelper.OVR+"Trying to connect to: "+str);

        if(ThinkingBar.instance)
            ThinkingBar.instance.Show("Connecting to: "+str);
        switch (deviceState)
        {
            case DeviceState.BLE_STATE:

                try
                {
                    helper.setDeviceName(str);
                    helper.Connect();
                    activeDeviceName = str;
                    return true;
                }
                catch (Exception e)
                {
                    //this is currently getting called when we try to restart
                    Debug.LogError(OVRHelper.OVR+e.ToString());
                    AutoConnect.instance.ScanForSerial();
                    return false;
                }
                break;
            case DeviceState.SERIAL_STATE:
                bool usbConnected = serialController.Connect(str);
                if (usbConnected)
                {
                    activeDeviceName = str;
                    return true;
                }
                else
                {
                    Debug.LogError(OVRHelper.OVR + "Unable to connect to Serial");
                    return false;
                }
                break;
        }
        return false;
    }


    void OnDataReceived(BluetoothHelper bluetoothHelperInstance)
    {
        OVRMessage.ReceiveMessage(helper.ReadBytes());
    }

    //BLE: event called after helper.ScanNearbyDevices() is initiated in StartScan();
    void OnScanEnded(BluetoothHelper bluetoothHelperInstance, LinkedList<BluetoothDevice> dList){
        this.devices = dList;
        if (devices.Count == 0 && connectedBLEDevices.Count == 0)
        {
            Debug.Log(OVRHelper.OVR+ "NO BLE Devices found!");
            if (isAutoConnect)
            {
                if (AutoConnect.instance)
                    AutoConnect.instance.ScanForSerial();
            }
            else
            {
                ConnectionSettings.instance.NoDevicesFound();
            }
        }
        else
        {
                BuildDeviceList();
        }
    }

    void OnConnectionFailed(BluetoothHelper bluetoothHelperInstance)
    {
        Debug.Log("OnConnectionFailed");
        isConnected = false;
        connectionType = "";

        //THIS MAY NEED WORK
        if (isAutoConnect)
        {
            if (AutoConnect.instance)
                AutoConnect.instance.Init();
        }
        else
        {
            ConnectionSettings.instance.NoDevicesFound();
        }


    }

    //all
    void OnConnected(BluetoothHelper bluetoothHelperInstance)
    {
        InitConnected();
    }

    void InitConnected()
    {
        
        isConnected = true;
        switch (deviceState)
        {
            case DeviceState.BLE_STATE:
                
                helper.StartListening();
                connectionType = "BLE";
                LocalStorage.SaveLastIon(activeDeviceName);
                
                //send initial message
                SetDeviceState();
                break;
            case DeviceState.SERIAL_STATE:
                if (helper != null)
                {
                    helper.Disconnect();
                }
                //reset
                serialTestCounter = 0;
                portNamesToTest = new List<string>();
                //end reset
                connectionType = "USB";
                break;
        }
        Debug.Log(OVRHelper.OVR + "Connected to: " + activeDeviceName);
    }

    public bool CheckForBle()
    {
        return helper.IsBluetoothEnabled();
    }

    void SetDeviceState()
    {
        //clear the message que first
        MessageQueueManager.ResetMessageQueue();
        //serialController.


        byte[] state = new byte[] { (byte)deviceState };
        ConnectionManager.instance.SendData(MessageTypes.DEVICE_SET_STATE, -1, state);
    }

    //BLE message recieved
    void OnMessageReceived()
    {
        OVRMessage.ReceiveMessage(helper.ReadBytes());
    }
    
    void BuildDeviceList()
    { 
        int deviceCount = 0;
        string deviceName="";
        List<string> ionNames = new List<string>();
        foreach (var device in devices)
        {
                
            if (device.DeviceName.Contains("ION") || device.DeviceName.Contains("ion"))
            {
                
                deviceCount++;
                deviceName = device.DeviceName;
                ionNames.Add(device.DeviceName);
                Debug.Log("deviceName:" + deviceName);
            }
        }

        if (ionNames.Count == 0)
        {
            Debug.Log(OVRHelper.OVR+"NO BLE Devices found!");
            if(isAutoConnect)
                AutoConnect.instance.ScanForSerial();
            else
            {
                ConnectionSettings.instance.NoDevicesFound();
            }
        }

        else if (ionNames.Count == 1)
        {
            Debug.Log(OVRHelper.OVR + "1 BLE Devices found!");
            if(isAutoConnect)
                AutoConnect.instance.ConnectToBLE(deviceName);
            else
                ConnectionSettings.instance.BuildBleButtons(ionNames);
        }
        else if (ionNames.Count > 1)
        {
            //multiple devices
            Debug.Log(OVRHelper.OVR + "Multiple BLE Devices found!");
            if (isAutoConnect) {
                bool connectedToLast = false;
                //connect to last if available
                foreach(string name in ionNames)
                {
                    if (name == LocalStorage.lastIon)
                    {
                        AutoConnect.instance.ConnectToBLE(deviceName);
                        connectedToLast = true;
                        break;
                    }
                }
                if (!connectedToLast)
                {
                    //if no last, just connect to first found
                    AutoConnect.instance.ConnectToBLE(ionNames[0]);
                }
           }
            else
            {
                ConnectionSettings.instance.BuildBleButtons(ionNames);
            }
        }
    }

    public void StartScan(bool isAuto = true)
    {
        Debug.Log(OVRHelper.OVR + "ConnectionManager: StartScan()");
        isAutoConnect = isAuto;
        helper.ScanNearbyDevices();
        if (ThinkingBar.instance)
            ThinkingBar.instance.Show("Scanning...");
        scanCounter = 0;        
    }

    public void Dissconnect(string serialPortToConnectTo = "")
    {
        Debug.Log("ConnectionManager: Dissconnect()");

        SendData(MessageTypes.DEVICE_SET_STATE,-1, new byte[] {0});
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            isConnected = false;
            connectionType = "";
            yield return new WaitForSeconds(0.5f);
            switch (deviceState)
            {
                case DeviceState.BLE_STATE:

                    if (helper != null)
                    {
                        helper.Disconnect();
                    }
                    yield return new WaitForSeconds(0.5f);
                    if (isAutoConnect)
                    {
                        AutoConnect.instance.Init();
                    }
                    break;
                case DeviceState.SERIAL_STATE:
                    if (serialController != null)
                        serialController.OnDisable();
                    yield return new WaitForSeconds(0.5f);
                    DisconnectSerial(serialPortToConnectTo);
                    break;
            }

           
        }


    }

    void DisconnectSerial(string serialPortToConnectTo="")
    {
        

        if (!isAutoConnect)
        {
            if (serialPortToConnectTo != "")
                CheckSingleSerial(serialPortToConnectTo);
            else
                ConnectionSettings.instance.UnableToConnect();
        }else{
            if(serialTestCounter < portNamesToTest.Count)
            {
                LoopThroughSerial(serialTestCounter);
            }
            else if(serialTestCounter > 0)
            {
                Debug.Log("tested all ports and they were bad");
                //try again
                Init(DeviceState.SERIAL_STATE);
            }
            else
            {
                Debug.Log("Just disconnected serial, we should try to autoconnect again");
                Invoke("InitWithDelay", 2f);
            }
        }
    }
    void InitWithDelay()
    {
        Init(DeviceState.SERIAL_STATE);
    }

    //all messages are created by calling SendData
    public void SendData(MessageTypes messageType)
    {
        byte[] data = null;
        byte[] packet = OVRMessage.CreateMessage(messageType, -1, data);
        MessageQueueManager.Add(packet);
    }
    public void SendData(MessageTypes messageType, int tube = -1)
    {
        byte[] data = null;
        byte[] packet = OVRMessage.CreateMessage(messageType, tube, data);
        MessageQueueManager.Add(packet);
    }
    public void SendData(MessageTypes messageType, int tube = -1, byte[] data = null, bool isTestData = false)
    {
        byte[] packet = OVRMessage.CreateMessage(messageType, tube, data);
        MessageQueueManager.Add(packet);  
    }
    public void SendData(MessageTypes messageType, int tube = -1, string str="", bool isTestData = false)
    {
        byte[] packet = OVRMessage.CreateMessage(messageType, tube, str);
        MessageQueueManager.Add(packet);
    }

    public void Send(byte[] packet)
    {
        /*if (ScreenManager.instance == null)
            return;
        if (ScreenManager.instance.activeScreen.name == "Screen_Test_Data")
        {
            ScreenManager.instance.activeScreen.GetComponent<MyScreen_Test_Data>().SetSentText(ByteHelper.CovertToReadableByteString(packet));
        }
        */

        //so instead of send ing the message we are going to push into the MessageQuw

        switch (deviceState)
        {
            case DeviceState.BLE_STATE:
                helper.SendData(packet);
                break;
            case DeviceState.SERIAL_STATE:
                serialController.SendSerialMessage(packet);
                break;
        }
    }

}
