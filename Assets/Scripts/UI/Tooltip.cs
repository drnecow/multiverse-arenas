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
    private float _backgroundSizeX;


    private void Awake()
    {
        _instance = this;
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
    }
    /*private void Update()
    {
        if (_instance.gameObject.activeSelf)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
            transform.localPosition = new Vector2(localPoint.x - _backgroundSizeX, localPoint.y);
        }   
    }*/

    public static void Show(string text)
    {
        _instance._text.text = text;
        Vector2 textSize = new Vector2(_instance._text.preferredWidth * 2f, _instance._text.preferredHeight * 2f);
        _instance._text.GetComponent<RectTransform>().sizeDelta = textSize;
        Vector2 backgroundSize = new Vector2(_instance._text.preferredWidth + 7 * 2f, _instance._text.preferredHeight + 7 * 2f);
        _instance._backgroundSizeX = backgroundSize.x;
        _instance._rectTransform.sizeDelta = backgroundSize;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_instance.transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        _instance.transform.localPosition = new Vector2(localPoint.x - _instance._backgroundSizeX, localPoint.y);

        _instance.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        _instance.gameObject.SetActive(false);
    }
}
