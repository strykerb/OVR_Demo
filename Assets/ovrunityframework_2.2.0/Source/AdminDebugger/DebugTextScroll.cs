using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugTextScroll : EventTrigger
{
    //TODO need to reset the scroll position when resizing

    private bool dragging;
    Vector2 offset = Vector2.zero;
    RectTransform rt;
    RectTransform parentRt;
    float prevY;

    //for scrolling
    public bool scrollingUp = false;
    public bool scrollingDown = false;
    float maxScrollSpeed = 20f;
    float defaultScrollSpeed = 1f;
    float scrollSpeedInc = 0.1f;
    float scrollSpeed;
    [SerializeField] AdminDebug adminDebug;
  



    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        parentRt = transform.parent.GetComponent<RectTransform>();
        prevY = rt.anchoredPosition.y;

    }

    public void UpdateScrollOnResize()
    {
        if (rt.rect.height < parentRt.rect.height)
        {
            LineToTop();
            return;
        }
        float diff;

        //diff = prevHeight - currentHeight
        //y - diff

    }
    public void OnScrollUpButtonDown()
    {
        scrollSpeed = defaultScrollSpeed;
        scrollingUp = true;
    }
    public void OnScrollUpButtonUp()
    {
        scrollingUp = false;
    }
    public void OnScrollDownButtonDown()
    {
        scrollSpeed = defaultScrollSpeed;
        scrollingDown = true;
    }
    public void OnScrollDownButtonUp()
    {
        scrollingDown = false;
    }


    void ScrollUp()
    {
        scrollSpeed += scrollSpeedInc;
        if (scrollSpeed > maxScrollSpeed) scrollSpeed = maxScrollSpeed;

        Vector2 adjPos;
        adjPos = rt.anchoredPosition;
        adjPos.y -= scrollSpeed;
        if (adjPos.y < 0) adjPos.y = 0;
        rt.anchoredPosition = adjPos;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
    void ScrollDown()
    {
        scrollSpeed += scrollSpeedInc;
        if (scrollSpeed > maxScrollSpeed) scrollSpeed = maxScrollSpeed;

        Vector2 adjPos;
        adjPos = rt.anchoredPosition;
        adjPos.y += scrollSpeed;
        /*if (adjPos.y > rt.rect.height - (adminDebug.newH-20))
            adjPos.y = rt.rect.height - (adminDebug.newH-20);*/
        rt.anchoredPosition = adjPos;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    public void LineToTop()
    {
        Vector2 adjPos;
        adjPos = rt.anchoredPosition;
        adjPos.y = 0;
        rt.anchoredPosition = adjPos;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
    public void LineToBottom()
    {
        if (rt.rect.height <= parentRt.rect.height) return;
        Vector2 adjPos= rt.anchoredPosition;
        float diff = rt.rect.height - parentRt.rect.height;
        adjPos.y = diff - 5;
        rt.anchoredPosition = adjPos;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
    public void Update()
    {
        if (scrollingUp) ScrollUp();
        if (scrollingDown) ScrollDown();
        if (dragging)
        {
            Vector2 newPos = new Vector2(transform.position.x, Input.mousePosition.y) - offset;
            Vector2 adjPos;
            if (rt.anchoredPosition.y < 0)
            {
                adjPos = rt.anchoredPosition;
                adjPos.y = 0;
                rt.anchoredPosition = adjPos;
                //LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
                Vector2 mousPos = new Vector2(transform.position.x, Input.mousePosition.y);
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                offset = mousPos - pos;
                return;

            }

           
            float diff = rt.rect.height - parentRt.rect.height;
            if (rt.anchoredPosition.y > diff)
            {
                adjPos = rt.anchoredPosition;
                adjPos.y = diff-5;
                rt.anchoredPosition = adjPos;
                //LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
                Vector2 mousPos = new Vector2(transform.position.x, Input.mousePosition.y);
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                offset = mousPos - pos;
                return;
            }
            transform.position = newPos;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            prevY = rt.anchoredPosition.y;





        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (rt.rect.height < parentRt.rect.height) return;
        Vector2 mousPos = new Vector2(transform.position.x, Input.mousePosition.y);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        offset = mousPos - pos;
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
