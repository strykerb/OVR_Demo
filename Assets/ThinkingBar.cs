using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThinkingBar : MonoBehaviour
{
    [SerializeField] GameObject anim;
    [SerializeField] TextMeshProUGUI tf;

    public static ThinkingBar instance;
    void Awake() { instance = this; }
    private void Start()
    {
        Hide();
    }

    public void Show(string str = "")
    {
        gameObject.SetActive(true);
        UpdateText(str);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateText(string str)
    {
        tf.text = str;
    }
}
