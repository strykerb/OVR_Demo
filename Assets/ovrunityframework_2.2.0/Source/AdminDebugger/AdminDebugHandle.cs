using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdminDebugHandle : EventTrigger
{
    private bool dragging;
    Vector2 offset = Vector2.zero;
    [SerializeField] RectTransform parentUI;
    Rect origRect;
    string handleDir = "";
    [SerializeField] RectTransform rightHandle;
    [SerializeField] RectTransform bottomHandle;
    [SerializeField] RectTransform cornerHandle;
    [SerializeField] GameObject handleIcon;
    Vector3 handleRot;

   [SerializeField] DebugTextScroll dts;




    public void Start()
    {
        origRect = parentUI.rect;
        handleDir = name;
        handleRot = handleIcon.GetComponent<RectTransform>().eulerAngles;
    }

    public void Update()
    {
        if (dragging)
        {
            Vector2 newPos;
            newPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - offset;
            handleIcon.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            switch (handleDir)
            { 
                case "Right":
                    newPos.y = transform.position.y;
                    transform.position = newPos;
                    parentUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transform.localPosition.x);
                    bottomHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transform.localPosition.x);
                    break;
                case "Bottom":
                    newPos.x = transform.position.x;
                    transform.position = newPos;
                    parentUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(transform.localPosition.y));
                    rightHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(transform.localPosition.y));
                    break;
                case "Corner":

                    transform.position = newPos;
                    parentUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(transform.localPosition.y));
                    rightHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(transform.localPosition.y));
                    parentUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transform.localPosition.x);
                    bottomHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transform.localPosition.x);
                    break;
            }
            dts.UpdateScrollOnResize();
           

        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Cursor.visible = false;
        Vector2 mousPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        offset = mousPos - pos;
        dragging = true;
        handleIcon.SetActive(true);
        switch (handleDir)
        {
            case "Right":
                handleRot.z = 0;
                break;
            case "Bottom":
                handleRot.z = 90;
                break;
            case "Corner":
                handleRot.z = -45;
                break;
        }
        handleIcon.GetComponent<RectTransform>().eulerAngles = handleRot;



    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        Cursor.visible = true;
        dragging = false;
        handleIcon.SetActive(false);
    }
}
