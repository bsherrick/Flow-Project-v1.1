using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject uiElement;

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiElement.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiElement.SetActive(false);
    }

}
