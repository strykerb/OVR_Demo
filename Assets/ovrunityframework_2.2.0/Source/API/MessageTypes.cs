
namespace OVR.API
{
    // Make sure this is compatible with Arduino
    // That means no fancy things like nested namespacing :[
    // The header byte structure is always 4 bytes long, 0x00 (checksum), 0x00 (messagetype IE: all of the below), 0x00 (Data Byte structure length), 0x00 (stop byte)
    // The data byte structure is a little more complicated.  
    // If the message pretains to a tube the first byte is always the tube designation 0x00 (tube), then everything after is data pretaining to that tube 0x00 (data).
    // If the message pretains to cartrdige or device the first and following bytes are pretaining to the message type 0x00 (data)
    // Data can be in the form of a single byte or designation of bytes following the header 0x00 (data), or it could be string data 0x00****** (string data)
    public enum MessageTypes
    {
        NONE = 0,                                       //00 - 0x00,  //
        INFO,                                           //01 - 0x01,  //0x00 0x01 0x00 0x00
        DEVICE_APP_CONNECT_RESPONSE,                    //02 - 0x02,  //0x00 0x02 0x00 0x00 
        DEVICE_APP_DISCONNECT_RESPONSE,                 //03 - 0x03,  //0x00 0x03 0x00 0x00

        FIRMWARE_VERSION_REQUEST,                       //04 - 0x0A,  //0x00 0x0A 0x00 0x00
        FIRMWARE_VERSION_RESPONSE,                      //05- 0x0B,  //0x00 0x0B 0x0x 0x00 0x00****** //"OVR v0.2.0.4" is an example of string

        DEVICE_NAME_REQUEST,                            //06 - 0x06,  //0x00 0x06 0x00 0x00
        DEVICE_NAME_RESPONSE,                           //07 - 0x07,  //0x00 0x07 0x0x 0x00 0x00******

        DEVICE_SERIAL_NUMBER_REQUEST,                   //08 - 0x08,  //0x00 0x08 0x00 0x00 
        DEVICE_SERIAL_NUMBER_RESPONSE,                  //09 - 0x09,  //0x00 0x09 0x0x 0x00 0x00******

        DEVICE_VERSION_REQUEST,                         //10 - 0x04,  //0x00 0x04 0x00 0x00
        DEVICE_VERSION_RESPONSE,                        //11 - 0x05,  //0x00 0x05 0x0x 0x00 0x00******

        DEVICE_BATTERY_LEVEL_REQUEST,                   //12 - 0x0C,  //0x00 0x0C 0x00 0x00
        DEVICE_BATTERY_LEVEL_RESPONSE,                  //13 - 0x0D,  //0x00 0x0D 0x01 0x00 0x00 //single byte 0-255

        DEVICE_STATE_REQUEST,                           //14 - 0x0E,  //0x00 0x0E 0x00 0x00
        DEVICE_STATE_RESPONSE,                          //15 - 0x0F,  //0x00 0x0F 0x01 0x00 0x0X //single byte 0-4
        //NO Response
        DEVICE_SET_STATE,                               //16 - 0x10,  //0x00 0x10 0x01 0x00 0x0X //single byte 0-4

        DEVICE_DEBUGMODE_REQUEST,                       //17 - 0x11,  //0x00 0x11 0x00 0x00
        DEVICE_DEBUGMODE_RESPONSE,                      //18 - 0x12,  //0x00 0x12 0x01 0x00 0x01 

        //NO Response
        ODORANT_COMMAND,                                //19 - 0x13,  //0xe3 0x13 0x04 0x00 0x00 0x01 0xff 0x00
        ODORANT_COMMANDS,                               //20 - 0x14,  //0xe3 0x14 0x08 0x00 0x00 0x01 0xff 0x00 0x00 0x02 0xff 0x00

        DEVICE_UPDATE_FIRMWARE_REQUEST,                 //21 - 0x15,  //0x00 0x15 0x00 0x00
        DEVICE_UPDATE_FIRMWARE_RESPONSE,                //22 - 0x16,  //0x00 0x16 0x01 0x00 0x01
        
        CARTRIDGE_BURST_COUNT_REQUEST,                  //23 - 0x17,  //0x00 0x17 0x00 0x00
        CARTRIDGE_BURST_COUNT_RESPONSE,                 //24 - 0x18,  //0x00 0x18 0x00 0x00 // 0x00 0xXX 0x04 0x00 0x00 0x00 0x00 0x10 by tube number (mData slot  3 bytes, 100,000 max)

        CARTRIDGE_S_MAX_T_REQUEST,                      //25 - 0x19,  //0x00 0x19 0x00 0x00// 0x00 0xXX 0x01 0x00 0x00 calibration screen (10000 is default) (0-25000) (0-8 as a byte) range 9000-11000, -5 = 9000, +5 =11000
        CARTRIDGE_S_MAX_T_RESPONSE,                     //26 - 0x1A,  //0x00 0x1A 0x04 0x00//tube, and 3 bytes of data
        
        //NO Response
        CARTRIDGE_SET_S_MAX_T,                          //27 - 0x1B,  //0x00 0x1B 0x00 0x00 // calibration screen 

        CARTRIDGE_SCENT_NAME_REQUEST,                   //28 - 0x1C,  //0x00 0x1C 0x00 0x00// 0x00 0xXX 0x04 0x00 0x00  device status screen
        CARTRIDGE_SCENT_NAME_RESPONSE,                  //29 - 0x1D,  //0x00 0x1D 0x00 0x00 //0x00 0xXX 0x04 0x00 0x00 0x00****** device status screen

        CARTRIDGE_FILL_DATE_REQUEST,                    //30 - 0x1E,  //0x00 0x1E 0x00 0x00// 0x00 0xXX 0x00 0x00 device status screen
        CARTRIDGE_FILL_DATE_RESPONSE,                   //31 - 0x1F,  //0x00 0x1F 0x00 0x00// 0x00 0xXX 0x04 0x00 0x00******  device status screen

        CARTRIDGE_SERIAL_NUMBER_REQUEST,                //32 - 0x20,  //0x00 0x20 0x00 0x00// 0x00 0xXX 0x00 0x0
        CARTRIDGE_SERIAL_NUMBER_RESPONSE,               //33 - 0x21,  //0x00 0x21 0x00 0x00 // 0x00 0xXX 0x04 0x00 0x00****** 

        CARTRIDGE_SCENT_PACK_NAME_REQUEST,              //34 - 0x22,  //0x00 0x22 0x00 0x00 // 0x00 0xXX 0x00 0x00 //IE: jungle_plants_1 or vignettes_2 
        CARTRIDGE_SCENT_PACK_NAME_RESPONSE,             //35 - 0x23,  //0x00 0x23 0x00 0x00 // 0x00 0xXX 0x04 0x00 0x00******

        //no responses
        //MFG TYPES - should remove from Framework
        //And only be in the APP
        MFG_SET_CARTRIDGE_BURST_COUNT,                  //36 - 0x24,  //0x00 0x24 0x00 0x00 //0x00 0xXX 0x04 0x00 0x00 0x00 0x00 0x10
        MFG_SET_CARTRIDGE_SCENT_NAMES,                  //37 - 0x25,  //0x00 0x25 0x00 0x00 //0x00 0xXX 0x04 0x00 0x00 0x00****** 
        MFG_SET_CARTRIDGE_FREQ,                         //38 - 0x26,  //0x00 0x26 0x00 0x00 //0x00 0xXX 0x02 0x00 0x00 0x00
        MFG_BURN_HASH,                                  //39 - 0x27,  //0x00 0x27 0x00 0x00 // TBD
        MFG_SET_DEVICE_NAME,                            //40 - 0x28,  //0x00 0x28 0x00 0x00 //0x00 0xXX 0x04 0x00 0x00****** 
        MFG_SET_DEVICE_VERSION,                         //41 - 0x29,  //0x00 0x29 0x00 0x00//0x00 0xXX 0x04 0x00 0x00****** 
        MFG_SET_CARTRIDGE_FILL_DATE,                    //42 - 0x2A,  //0x00 0x2A 0x00 0x00//0x00 0xXX 0x04 0x00 0x00******
        MFG_SET_CARTRIDGE_SCENT_PACK_NAME,              //43 - 0x2B,  //0x00 0x2B 0x00 0x00//0x00 0xXX 0x04 0x00 0x00******
        MFG_SET_CARTRIDGE_SERIAL_NUMBER,                //44 - 0x2C,  //0x00 0x2C 0x00 0x00//0x00 0xXX 0x04 0x00 0x00******
        MFG_SET_DEVICE_SERIAL_NUMBER,                   //45 - 0x2D
        SET_DEVICE_WIFI_CREDENTIALS,                    //46 - 0x2E
        MFG_SET_CARTRIDGE_T_MAX_T,                      //47 - 2F,  //00 2F 04 00 XX XX XX XX
        MFG_CARTRIDGE_T_MAX_T_REQUEST,                  //48 - 30,  //00 30 01 00 XX //only pass in the slot/tube number
        MFG_CARTRIDGE_T_MAX_T_RESPONSE,                 //49 - 31,  //00 31 04 00 XX XX XX XX //returns the tube number and the 3 byte representation of the T_MAX_T
        MFG_CARTRIDGE_FREQ_REQUEST,                     //50 - 32,  //00 32 01 00 XX //only pass in the slot/tube number
        MFG_CARTRIDGE_FREQ_RESPONSE                     //51 - 33,  //00 33 04 00 XX XX XX XX //returns the tube number and the 3 byte representation of the frequency
    }

    public enum DeviceState
    {
        NO_STATE = 0,                //0 - 0x00               
        SERIAL_STATE,                //1 - 0x01
        BLE_STATE,                   //2 - 0x02               
        BT_STATE,                    //3 - 0x03   
        OTG_State                    //4 - 0x04
        // WIFI_STATE                //5 - 0x05
    };
}
