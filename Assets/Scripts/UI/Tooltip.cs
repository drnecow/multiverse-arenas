using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    private static Tooltip _instance;

    private TextMeshProUGUI _text;
    private RectTransform _rectTransform;
    private RectTransform _initialParent;


    private void Awake()
    {
        _instance = this;
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
        _initialParent = _rectTransform.parent.GetComponent<RectTransform>();
    }
    /*private void Update()
    {
        if (_instance.gameObject.activeSelf)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
            transform.localPosition = localPoint;
        }   
    }*/

    public static void Show(string text, RectTransform parent)
    {
        _instance._text.text = text;
        _instance.transform.SetParent(parent);
        _instance._rectTransform.localPosition = new Vector2(parent.sizeDelta.x / 2, -(parent.sizeDelta.y / 2));
        _instance.transform.SetParent(_instance._initialParent);

        _instance.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        _instance.gameObject.SetActive(false);
    }
}
