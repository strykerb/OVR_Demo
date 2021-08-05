using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdminDebug : MonoBehaviour
{
    [SerializeField] RectTransform rightHandle;
    [SerializeField] RectTransform bottomHandle;


    public TextMeshProUGUI text;
    public static TextMeshProUGUI sText;//just static ref to the text
    public  DebugTextScroll dts;
    public static DebugTextScroll sdts;

    public TextMeshProUGUI testDataText;
    public static TextMeshProUGUI sTestDataText;//just static ref to the data
    public DebugTextScroll debugTextScroll;

    static string logText;

    public float newW;
    public float newH;

    public static AdminDebug instance;
    private void Awake()
    {
        sText = text;
        sdts = dts;
        sTestDataText = testDataText;
        instance = this;
        gameObject.SetActive(false);


    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Open()
    {
        SizeAndCenter();
        gameObject.SetActive(true);
        sText.text = logText;
        sdts.LineToTop();
        sdts.LineToBottom();
        //HamburgerMenu.instance.Toggle();
    }
    public void SizeAndCenter()
    {
        newW = transform.parent.GetComponent<RectTransform>().sizeDelta.x;//500
        newH = transform.parent.GetComponent<RectTransform>().sizeDelta.y/2;// 500;

        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newW);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newH);

        bottomHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newW);
        rightHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newH);

        GetComponent<RectTransform>().ForceUpdateRectTransforms();



        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -newH);

        debugTextScroll.LineToBottom();
    }
    public void Clear()
    {
        logText = "";
        text.text = "";
        //need to reset position
        sdts.LineToTop();
    }
    public void HandleLineToTop()
    {
        debugTextScroll.LineToTop();
    }
    public static void Print(string str, PrintState printState = PrintState.MESSAGE)
    {
        string testText = "";
        switch (printState)
        {
            case PrintState.REQUEST:
                testText = "<color=#D8E21B> >>REQUEST: " + str + "</color>\n";
                break;
            case PrintState.RESPONSE:
                testText = "<color=#53B0E2> >>RESPONSE: " + str + "</color>\n";
                break;
            case PrintState.ERROR:
                testText = "<color=#FF0000>ERROR: " + str + "</color>\n";
                break;
            case PrintState.MESSAGE:
                testText = "<color=#FFFFFF>" + str + "</color>\n";
                break;
            case PrintState.TIME:
                testText = "<color=#00FF00>TIME: " + str + "</color>\n";
                break;
        }
        logText += testText;
        if (sText != null)
        {
            sText.text = logText;
        }
    }
}
public enum PrintState
{
    REQUEST = 0,
    RESPONSE = 1,
    ERROR = 2,
    MESSAGE = 3,
    TIME = 4
};
