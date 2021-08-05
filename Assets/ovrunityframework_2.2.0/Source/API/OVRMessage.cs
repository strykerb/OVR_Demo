using System.Collections;
using System.Collections.Generic;
using OVR.API;
using OVR.Data;
using UnityEngine;
using System;

namespace OVR.API
{
    public class OVRMessage
    {
        
        public static byte[] CreateMessage(MessageTypes messageType, int tube = -1, string str="")
        {
            Debug.Log("NEW CreateMessage, string passed in");
            //byte[] bytes = new byte[0];
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

            return CreateMessage(messageType, tube, bytes);
        }

        public static byte[] CreateMessage(MessageTypes messageType, int tube = -1, byte[] paramData=null)
        {
            Debug.Log(OVRHelper.OVR+Enum.GetName(typeof(MessageTypes), messageType));

            byte[] messHeader = new byte[0];
            byte[] dataBytes = null;
            switch (messageType)
            {


                //new
                case MessageTypes.SET_DEVICE_WIFI_CREDENTIALS:
                    messHeader = new byte[4 + paramData.Length];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = (byte)paramData.Length;
                    messHeader[3] = 0x00;
                    for (int i = 0; i < paramData.Length; i++)
                    {
                        messHeader[4 + i] = paramData[i];
                    }
                    break;
                //default settings
                case MessageTypes.DEVICE_UPDATE_FIRMWARE_REQUEST:
                case MessageTypes.DEVICE_SET_STATE://2 for BLE   
                    messHeader = new byte[5];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = 0x01;
                    messHeader[3] = 0x00;
                    if (paramData == null)
                        messHeader[4] = 0x02;
                    else messHeader[4] = paramData[0];
                    break;
                //default settings
                case MessageTypes.MFG_SET_CARTRIDGE_SCENT_PACK_NAME:
                case MessageTypes.MFG_SET_DEVICE_NAME:
                case MessageTypes.MFG_SET_CARTRIDGE_FILL_DATE:
                case MessageTypes.MFG_SET_DEVICE_VERSION:
                case MessageTypes.MFG_SET_CARTRIDGE_SERIAL_NUMBER:
                case MessageTypes.MFG_SET_DEVICE_SERIAL_NUMBER:
                    if (paramData==null)
                    {
                        if (messageType == MessageTypes.MFG_SET_DEVICE_SERIAL_NUMBER)
                        {
                            dataBytes = System.Text.Encoding.UTF8.GetBytes("sn12345");
                        }
                        if(messageType==MessageTypes.MFG_SET_DEVICE_VERSION)
                        {
                            dataBytes = System.Text.Encoding.UTF8.GetBytes("1.1.0");
                        }
                        else if (messageType == MessageTypes.MFG_SET_CARTRIDGE_SERIAL_NUMBER)
                        {
                            dataBytes = System.Text.Encoding.UTF8.GetBytes("cart12345");
                        }
                        else if(messageType== MessageTypes.MFG_SET_CARTRIDGE_SCENT_PACK_NAME)
                        {
                            dataBytes = System.Text.Encoding.UTF8.GetBytes("Wellnesee_1");
                        }
                        else if (messageType == MessageTypes.MFG_SET_DEVICE_NAME)
                        {
                            dataBytes = System.Text.Encoding.UTF8.GetBytes("ION_Joe_Is_Cool");
                        }
                        else if (messageType == MessageTypes.MFG_SET_CARTRIDGE_FILL_DATE)
                        {
                            DateTime thisDay = DateTime.Today;
                            string dateStr = thisDay.ToString("d");
                            dataBytes = System.Text.Encoding.UTF8.GetBytes(dateStr);
                        }
                    }
                    else
                    {
                        dataBytes = paramData;
                        //need to figure out paramData that would be passed from test
                    }
                    messHeader = new byte[4 + dataBytes.Length];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = (byte)dataBytes.Length;
                    messHeader[3] = 0x00;
                    for (int i = 0; i < dataBytes.Length; i++)
                    {
                        messHeader[4 + i] = dataBytes[i];
                    }
                    break;
                //default settings
                case MessageTypes.MFG_SET_CARTRIDGE_BURST_COUNT:
                    dataBytes = ByteHelper.ConvertIntTo3Bytes(0);
                    messHeader = new byte[8];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = 0x04;
                    messHeader[3] = 0x00;
                    messHeader[4] = (byte)tube;//1 byte
                    for (int i = 0; i < dataBytes.Length; i++)
                    {
                        messHeader[5 + i] = dataBytes[i];
                    }
                    break;
                //default settings
                case MessageTypes.MFG_SET_CARTRIDGE_SCENT_NAMES:
                    messHeader = new byte[5+paramData.Length];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = (byte)(paramData.Length+1);
                    messHeader[3] = 0x00;
                    messHeader[4] = (byte)tube;//1 byte
                    for (int i = 0; i < paramData.Length; i++)
                    {
                        messHeader[5 + i] = paramData[i];
                    }
                    break;
                case MessageTypes.CARTRIDGE_SET_S_MAX_T:
                    messHeader = new byte[6];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = (byte)(paramData.Length + 1);
                    messHeader[3] = 0x00;
                    messHeader[4] = (byte)tube;//1 byte
                    messHeader[5] = paramData[0];
                    break;
                case MessageTypes.MFG_SET_CARTRIDGE_T_MAX_T://sending 3 bytes
                    messHeader = new byte[8];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = (byte)(paramData.Length + 1);
                    messHeader[3] = 0x00;
                    messHeader[4] = (byte)tube;//1 byte
                    //messHeader[5] = paramData[0];
                    for (int i = 0; i < paramData.Length; i++)
                    {
                        messHeader[5 + i] = paramData[i];
                    }
                    break;
                /*
                case MessageTypes.CARTRIDGE_SET_S_MAX_T://sending 1 bytes
                    messHeader = new byte[6];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = (byte)(paramData.Length + 1);
                    messHeader[3] = 0x00;
                    messHeader[4] = (byte)tube;//1 byte
                    messHeader[5] = paramData[0];
                    break;*/
                //added 3/10/21
                case MessageTypes.MFG_SET_CARTRIDGE_FREQ:
                    messHeader = new byte[8];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = (byte)(paramData.Length + 1);
                    messHeader[3] = 0x00;
                    messHeader[4] = (byte)tube;//1 byte
                    for (int i = 0; i < paramData.Length; i++)
                    {
                        messHeader[5 + i] = paramData[i];
                    }
                    break;
                //requests
                case MessageTypes.DEVICE_VERSION_REQUEST:
                case MessageTypes.DEVICE_NAME_REQUEST:
                case MessageTypes.DEVICE_SERIAL_NUMBER_REQUEST:
                case MessageTypes.FIRMWARE_VERSION_REQUEST:
                case MessageTypes.DEVICE_BATTERY_LEVEL_REQUEST:
                case MessageTypes.DEVICE_STATE_REQUEST:
                case MessageTypes.CARTRIDGE_FILL_DATE_REQUEST:
                case MessageTypes.CARTRIDGE_SERIAL_NUMBER_REQUEST:
                case MessageTypes.CARTRIDGE_SCENT_PACK_NAME_REQUEST:
                    messHeader = new byte[4];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = 0x00;
                    messHeader[3] = 0x00;
                    break;

                //TODO DEVICE_DEBUGMODE_REQUEST - TODO
                //case 17:
                //   break;
                //requests
                case MessageTypes.CARTRIDGE_BURST_COUNT_REQUEST:
                case MessageTypes.CARTRIDGE_S_MAX_T_REQUEST:
                case MessageTypes.CARTRIDGE_SCENT_NAME_REQUEST:
                case MessageTypes.MFG_CARTRIDGE_FREQ_REQUEST://new 7/1
                case MessageTypes.MFG_CARTRIDGE_T_MAX_T_REQUEST://new 7/1
                    messHeader = new byte[5];
                    messHeader[0] = 0x00;
                    messHeader[1] = (byte)messageType;
                    messHeader[2] = 0x01;
                    messHeader[3] = 0x00;
                    messHeader[4] = (byte)tube;
                    break;
                case MessageTypes.ODORANT_COMMANDS:
                    int headerSize = 4 + (OdorantManager.instance._commands.Count* OdorantCommand.Size);
                    messHeader = new byte[headerSize];
                    int index = 4;
                    messHeader[0] = 0xe3;
                    messHeader[1] = (byte)MessageTypes.ODORANT_COMMANDS;
                    messHeader[2] = (byte)(OdorantManager.instance._commands.Count * 4);
                    messHeader[3] = 0x00;
                    //loop through the commands
                    foreach (var command in OdorantManager.instance._commands)
                    {
                        Array.Copy(command.GetPacket(), 0, messHeader, index, OdorantCommand.Size);
                        index += OdorantCommand.Size;
                    }
                    break;
                default:
                    Debug.Log("Not a valid case");
                    break;
            }
            string hexStr = ByteHelper.CovertToReadableByteString(messHeader);
            Debug.Log(OVRHelper.OVR+ "Message Sent: " + hexStr);

            return messHeader;
        }

        public static void ReceiveMessage(byte[] buff)
        {
            Debug.Log(OVRHelper.OVR+"OVRMessage:RecieveMessage");
            /*
            //if test data, poplate the return field
            if (ScreenManager.instance)
            {
                if (ScreenManager.instance.activeScreen.name == "Screen_Test_Data")
                {
                    ScreenManager.instance.activeScreen.GetComponent<MyScreen_Test_Data>().SetResultText(ByteHelper.CovertToReadableByteString(buff));
                }
            }*/

            //bad messages
            if (buff.Length < 4)
            {
                Debug.Log("Bad Message 1: <4");
                /*Debug.Log(ByteHelper.CovertToReadableByteString(buff));
                Debug.Log(System.Text.Encoding.ASCII.GetString(buff));
                AdminDebug.Print("Bad Message Recieved, less than 4:" + " : " + ByteHelper.CovertToReadableByteString(buff), PrintState.ERROR);
                AdminDebug.Print(System.Text.Encoding.ASCII.GetString(buff), PrintState.MESSAGE);
                */
                return;
            }
            MessageTypes messageType = MessageTypes.NONE;
            try
            {
                messageType = (MessageTypes)buff[1];
                if (messageType == MessageTypes.INFO)
                    return;
            }
            catch (Exception e)
            {
                Debug.Log("Bad Message 2: not a valid type");
                /*Debug.Log(ByteHelper.CovertToReadableByteString(buff));
                Debug.Log(System.Text.Encoding.ASCII.GetString(buff));
                AdminDebug.Print("Bad Message Recieved, not valid type:" + " : " + ByteHelper.CovertToReadableByteString(buff), PrintState.ERROR);
                AdminDebug.Print(System.Text.Encoding.ASCII.GetString(buff), PrintState.MESSAGE);
                */
                return;
            }
            if(buff[3] != 0)
            {
                Debug.Log("Bad Message 3: header does not end in 0");
                /*Debug.Log(ByteHelper.CovertToReadableByteString(buff));
                Debug.Log(System.Text.Encoding.ASCII.GetString(buff));
                AdminDebug.Print("Bad Message Recieved, Not ending 0:" + " : " + ByteHelper.CovertToReadableByteString(buff), PrintState.ERROR);
                AdminDebug.Print(System.Text.Encoding.ASCII.GetString(buff), PrintState.MESSAGE);
                */
                return;
            }
            if (buff.Length > 40)
            {
                Debug.Log("Bad Message 4: buff.Length > 40");
                return;
            }
                //check for


                MessageQueueManager.HandleResponse(buff);

            //AdminDebug.Print(Enum.GetName(typeof(MessageTypes), messageType) + " : " + ByteHelper.CovertToReadableByteString(buff), PrintState.RESPONSE);
            Debug.Log(OVRHelper.OVR+ Enum.GetName(typeof(MessageTypes), messageType) + " : " + ByteHelper.CovertToReadableByteString(buff));
            int lengthOfData = buff.Length - 4;// buff[2];

            byte[] data = new byte[lengthOfData];

            int tube = -1;
            if (lengthOfData > 0)
            {
                tube = (int)buff[4];
                if (buff.Length > 4)
                {
                    Array.Copy(buff, 4, data, 0, lengthOfData);
                }
            }
            string stringData = "";
            int returnInt = 0;

            switch (messageType)
            {
                case MessageTypes.INFO:
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    break;

                //not sure what to do with this in the app yet
                case MessageTypes.DEVICE_DEBUGMODE_RESPONSE:
                    break;
                case MessageTypes.DEVICE_VERSION_RESPONSE:  
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalDeviceVersion(stringData);
                    break;
                case MessageTypes.DEVICE_STATE_RESPONSE:
                    returnInt = (int)data[0];
                    AdminDebug.Print("Devoce State: " + returnInt, PrintState.MESSAGE);
                    //DeviceStatus.instance.SetDeiviceState(returnInt);
                    break;
                case MessageTypes.DEVICE_UPDATE_FIRMWARE_RESPONSE:
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    bool isUpdate = Convert.ToBoolean(buff[4]);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    //DeviceStatus.instance.SetDeviceUpdate(isUpdate);
                    break;
                case MessageTypes.FIRMWARE_VERSION_RESPONSE:
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalFirmwareVersion(stringData);
                    break;
                case MessageTypes.CARTRIDGE_FILL_DATE_RESPONSE:
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    ///DeviceStatus.instance.SaveLocalCartridgeFillDate(stringData);
                    break;
                case MessageTypes.CARTRIDGE_SERIAL_NUMBER_RESPONSE:
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalCartridgeSerial(stringData);
                    break;
                case MessageTypes.DEVICE_SERIAL_NUMBER_RESPONSE://
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalDeviceSerial(stringData);
                    break;
                case MessageTypes.DEVICE_NAME_RESPONSE://
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    ConnectionManager.instance.serialTestState = "success";
                    //DeviceStatus.instance.SetDeviceName(stringData);
                    break;
                case MessageTypes.CARTRIDGE_SCENT_PACK_NAME_RESPONSE:
                    stringData = System.Text.Encoding.ASCII.GetString(data);
                    AdminDebug.Print(stringData, PrintState.MESSAGE);
                    //DeviceStatus.instance.SetScentPackName(stringData);
                    break;
                case MessageTypes.DEVICE_BATTERY_LEVEL_RESPONSE://
                    byte[] batteryLevel = new byte[4];
                    Array.Copy(data, 0, batteryLevel, 0, 4);
                    Array.Reverse(batteryLevel);
                    returnInt = BitConverter.ToInt32(batteryLevel, 0);
                    AdminDebug.Print("Battery Level: "+ returnInt, PrintState.MESSAGE);
                    //DeviceStatus.instance.SetBatteryLevel(returnInt);
                    break;
                //**************************
                //requests 1 tube
                //**************************
                case MessageTypes.CARTRIDGE_BURST_COUNT_RESPONSE:
                    byte[] burstData = new byte[3];
                    Array.Copy(data, 1, burstData, 0, 3);
                    returnInt = ByteHelper.Convert3BytesToInt(burstData);
                    AdminDebug.Print("Tube: " + tube + ", bursts=" + returnInt, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalBurstCount(tube, returnInt);
                    break;
                case MessageTypes.CARTRIDGE_S_MAX_T_RESPONSE:
                    byte[] maxTData = new byte[1];
                    try
                    {
                        Array.Copy(data, 1, maxTData, 0, 1);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    returnInt = (int)maxTData[0];
                    AdminDebug.Print("Tube: " + tube + ", maxT=" + returnInt, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalSMaxT(tube, returnInt);
                    break;
                case MessageTypes.MFG_CARTRIDGE_FREQ_RESPONSE:
                    byte[] freqData = new byte[3];
                    try
                    {
                        Array.Copy(data, 1, freqData, 0, 3);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    returnInt = ByteHelper.Convert3BytesToInt(freqData);
                    AdminDebug.Print("Tube: " + tube + ", frequency=" + returnInt, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalFreq(tube, returnInt);
                    break;
                case MessageTypes.MFG_CARTRIDGE_T_MAX_T_RESPONSE:
                    byte[] tMaxTData = new byte[3];
                    try
                    {
                        Array.Copy(data, 1, tMaxTData, 0, 3);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    returnInt = ByteHelper.Convert3BytesToInt(tMaxTData);
                    AdminDebug.Print("Tube: " + tube + ", tMaxT=" + returnInt, PrintState.MESSAGE);
                    //DeviceStatus.instance.SaveLocalTMaxT(tube, returnInt);
                    break;
                case MessageTypes.CARTRIDGE_SCENT_NAME_RESPONSE:
                    byte[] scentNameData = new byte[lengthOfData - 1];
                    Array.Copy(data, 1, scentNameData, 0, lengthOfData - 1);
                    stringData = System.Text.Encoding.ASCII.GetString(scentNameData);
                    AdminDebug.Print("Tube: " + tube + ", name=" + stringData, PrintState.MESSAGE);
                    //DeviceStatus.instance.SetTubeName(tube, stringData);
                    break;
                case MessageTypes.ODORANT_COMMAND:
                case MessageTypes.ODORANT_COMMANDS:
                    break;
                default:
                    Debug.Log("Not a valid case");
                    break;
            }
            /*
            //set the readable result for test data screen
            if (ScreenManager.instance.activeScreen.name == "Screen_Test_Data")
            {
                string readableResult = "";
                switch (messageType)
                {
                    case MessageTypes.CARTRIDGE_SCENT_NAME_RESPONSE:
                        readableResult = "Tube: " + tube + ", name=" + stringData;
                        break;
                    //case MessageTypes.CARTRIDGE_S_MAX_T_RESPONSE:
                    case MessageTypes.CARTRIDGE_S_MAX_T_RESPONSE:
                        readableResult = "Tube: " + tube + ", maxT=" + returnInt;
                        break;
                    case MessageTypes.CARTRIDGE_BURST_COUNT_RESPONSE:
                        readableResult = "Tube: " + tube + ", bursts=" + returnInt;
                        break;
                    case MessageTypes.DEVICE_BATTERY_LEVEL_RESPONSE:
                        readableResult = "Battery Level: " + returnInt;
                        break;
                    case MessageTypes.DEVICE_STATE_RESPONSE:
                        readableResult = "Device State: " + returnInt;
                        break;
                    default:
                        readableResult = stringData;
                        break;
                }
                ScreenManager.instance.activeScreen.GetComponent<MyScreen_Test_Data>().SetReadableResultText(readableResult);
            }*/
        }
    }
}





