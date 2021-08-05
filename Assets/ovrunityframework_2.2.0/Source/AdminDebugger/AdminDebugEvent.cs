using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdminDebugEvent : EventTrigger
{
    private bool dragging;
    Vector2 offset = Vector2.zero;
    

    public void Update()
    {
        if (dragging)
        {

            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)-offset;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
        Vector2 mousPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        offset = mousPos - pos;
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
