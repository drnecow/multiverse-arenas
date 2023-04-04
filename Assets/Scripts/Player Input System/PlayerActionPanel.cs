using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] protected GameObject _buttonPrefab;
    [SerializeField] protected RectTransform _buttonsParent;

    protected List<Button> _buttons;
    protected Monster _actor;


    private void Awake()
    {
        _buttons = new List<Button>();
    }

    public virtual void SetAllButtonsInteractabilityByCondition()
    {

    }
    public void SetAllButtonsNonInteractable()
    {
        foreach (Button button in _buttons)
            button.interactable = false;
    }
    public void DestroyAllButtons()
    {
        Debug.Log($"Number of buttons: {_buttons.Count}");

        foreach (Button button in _buttons)
        {
            Debug.Log(button.gameObject);
            Destroy(button.gameObject);
        }

        _buttons.Clear();
    }
}
