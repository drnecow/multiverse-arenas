using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] protected List<GameObject> _actionSlots;
    [SerializeField] protected VisualAssets _visuals;
    [SerializeField] private Color _initialColor;
    [SerializeField] protected TextDescriptions _descriptions;
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
            actionSlot.GetComponent<Image>().color = _initialColor;
            actionSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";
            actionSlot.GetComponent<TooltipTarget>().Enabled = false;

            Button actionSlotButton = actionSlot.GetComponent<Button>();
            actionSlotButton.enabled = false;           
        }
    }
}
