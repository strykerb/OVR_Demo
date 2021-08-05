using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVR.API
{
    public class AutoConnect : MonoBehaviour
    {
        private float maintainConnectionTimer = 0.5f;
        private float maintainConnectionTimerInterval = 1.0f;
        public static AutoConnect instance;
        public AutoConnectType autoConnectType;

        private void Awake() { instance = this;}

        public void SetAutoConnectionType(AutoConnectType act)
        {
            autoConnectType = act;
        }
        public void Init()
        {
            Debug.Log(OVRHelper.OVR + "AutoConnect: Init()");
            Invoke("InitDelay", 1f);
        }

        void InitDelay()
        {
            if (!ConnectionManager.instance.isConnected)
            {
                ConnectionManager.instance.isAutoConnect = true;
                if (CanScanForBLE())
                    ScanForBle();
                else if (CanScanForSerial())
                    ScanForSerial();
                else
                {
                    Debug.Log(OVRHelper.OVR + "No Connecections to try (this should not happen)");
                    HandleNoConnectionAvailable();
                }
            }
            else
            {
                Debug.Log(OVRHelper.OVR + "Already connected to: " + ConnectionManager.instance.activeDeviceName);
            }
        }
        //BLE
        void ScanForBle()
        {
            if (CanScanForBLE())
            {
                StartCoroutine(Delay());
                IEnumerator Delay()
                {
                    if (Application.platform.ToString().ToLower().Contains("window"))
                    {
                        Debug.Log(OVRHelper.OVR + "Windows currrently does not support BLE");
                        if (CanScanForSerial())
                            ScanForSerial();
                    }
                    //Connect with BLE
                    else
                    {
                        //init the BLE State
                        ConnectionManager.instance.Init(DeviceState.BLE_STATE);
                        yield return new WaitForSeconds(0.5f);
                        if (ConnectionManager.instance.CheckForBle())
                        {
                            Debug.Log(OVRHelper.OVR + "BLE is on:)");
                            ConnectionManager.instance.StartScan(true);
                        }
                        //if BLE is off, pop up warning
                        ////need to tell person ble is off and
                        //to turn it on
                        else
                        {
                            Debug.Log(OVRHelper.OVR + "BLE is off:(");
                            //try serial
                            if (CanScanForSerial())
                            {
                                ScanForSerial();
                            }
                            else
                            {
                                HandleNoConnectionAvailable();
                            }
                        }
                    }
                }
            }
        }
        public void ConnectToBLE(string deviceName)
        {
            StartCoroutine(Delay());
            //trying to Connect
            IEnumerator Delay()
            {
                if (deviceName == LocalStorage.GetLastIon())
                {
                    Debug.Log(OVRHelper.OVR + "Last Connected Ion Device found: " + deviceName);
                }
                else
                    Debug.Log(OVRHelper.OVR + "NEW Ion Device found: " + deviceName);
                yield return new WaitForSeconds(1.0f);
                ConnectionManager.instance.Connect(deviceName);
            }
        }

        public void ConnectToSerial(string deviceName)
        {

        }


        public void HandleNoConnectionAvailable()
        {
            ConnectionSettings.instance.NoDevicesFound();
            Debug.Log(OVRHelper.OVR + "No Connection Available.");
            Debug.Log(OVRHelper.OVR + "--------- Retrying in 2 seconds------------");
            Invoke("Init", 2f);
        }

        public void ScanForSerial()
        {

            if (CanScanForSerial())
            {
                Debug.Log(OVRHelper.OVR + "ScanForSerial");
                ConnectionManager.instance.Init(DeviceState.SERIAL_STATE);
            }
            else
                HandleNoConnectionAvailable();
        }

        bool CanScanForBLE()
        {
            if (autoConnectType == AutoConnectType.ANY || autoConnectType == AutoConnectType.BLE_ONLY)
                return true;
            else
                return false;
        }
        bool CanScanForSerial()
        {
            if (autoConnectType == AutoConnectType.ANY || autoConnectType == AutoConnectType.SERIAL_ONLY)
                return true;
            else
                return false;
        }
    }
}

public enum AutoConnectType
{
    ANY = 0,                //0 - 0x00               
    BLE_ONLY,                //1 - 0x01
    SERIAL_ONLY
};