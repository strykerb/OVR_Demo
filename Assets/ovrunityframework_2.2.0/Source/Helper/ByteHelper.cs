using System.Collections;
using System.Collections.Generic;
using OVR.API;


public static class ByteHelper
{

    //converts an int ot 3 bytes
    public static byte[] ConvertIntTo3Bytes(int value)
    {
        byte[] result = new byte[] {
            (byte) ((value >> 16) & 0xFF),
            (byte) ((value >> 8) & 0xFF),
            (byte) (value & 0xFF)
            /*
            (byte) (value & 0xFF),
            (byte) ((value >> 8) & 0xFF),
            (byte) ((value >> 16) & 0xFF)*/
        };
        return result;
    }
    //converts a 3 byte array to an int
    public static int Convert3BytesToInt(byte[] value)
    {
        int result = value[0] << 16 | value[1] << 8 | value[2];
        //int result = value[2] << 16 | value[1] << 8 | value[0];

        return result;
    }
    //makes a byte array readable hex values
    public static string CovertToReadableByteString(byte[] value)
    {
        string hexStr = "";
        for (int i = 0; i < value.Length; i++)
        {
            byte b = value[i];
            var bStr = b.ToString("X");
            if (bStr.Length == 1) bStr = "0x0" + bStr;
            else bStr = "0x" + bStr;
            hexStr += bStr;
            if (i + 1 < value.Length)
            {
                hexStr += ", ";
            }
        }
        return hexStr;
    }

}
