using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] protected List<GameObject> _actionSlots;
    protected List<Button> _buttons;

    protected Monster _actor;
    protected PlayerInputSystem _parentInputSystem;


    public void SetParentInputSystem(PlayerInputSystem parentInputSystem)
    {
        _parentInputSystem = parentInputSystem;
    }
    public virtual void SetAllButtonsInteractabilityByCondition()
    {

    }
    public void SetAllButtonsNonInteractable()
    {
        foreach (Button button in _buttons)
            button.interactable = false;
    }
    public void ClearAllButtons()
    {
        foreach (Button button in _buttons)
            button.onClick.RemoveAllListeners();

        _buttons.Clear();

        foreach (GameObject actionSlot in _actionSlots)
        {
            actionSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";

            Button actionSlotButton = actionSlot.GetComponent<Button>();
            actionSlotButton.enabled = false;
        }
    }
}
