using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string _text = "Test text";
    private RectTransform _rectTransform;

    private bool _enabled = false;
    public bool Enabled { get => _enabled; set => _enabled = value; }


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_enabled)
            Tooltip.Show(_text, _rectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide();
    }

    public void SetText(string text)
    {
        _text = text;
    }
}
