using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapMouceOverEvent : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private GameObject overImg;

    public void OnPointerEnter(PointerEventData eventData)
    {
        overImg.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        overImg.SetActive(false);
    }

    public void SetOverImgOff()
    {
        overImg.SetActive(false);
    }
}
