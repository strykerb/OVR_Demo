using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WifiInfo
{
    public string ssid;
    public string pw;

    //store the scent info locally
    public WifiInfo(string ssid, string pw)
    {
        this.ssid = ssid;
        this.pw = pw;
    }
}


public static class LocalStorage
{
    public static string lastIon;
    

    public static void SaveWifi(string ssid, string wifiPw)
    {
        PlayerPrefs.SetString("ssid", ssid);
        PlayerPrefs.SetString("wifiPw", wifiPw);
    }
    public static WifiInfo GetWifi()
    {
        WifiInfo info = new WifiInfo(PlayerPrefs.GetString("ssid"), PlayerPrefs.GetString("wifiPw"));
        return info;
    }
    public static void SaveLastIon(string ionName)
    {
        PlayerPrefs.SetString("lastIon", ionName);
    }
    public static string GetLastIon()
    {
        return PlayerPrefs.GetString("lastIon");
    }
}
