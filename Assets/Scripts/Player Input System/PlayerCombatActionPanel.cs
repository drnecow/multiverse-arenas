using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Constants;

public class PlayerCombatActionPanel : PlayerActionPanel
{
    private List<CombatAction> _combatActions;
    private CombatActionType _combatActionType;


    public void CreateButtons(List<CombatAction> combatActions, CombatActionType combatActionType, Monster actor, PlayerInputSystem playerInputSystem)
    {
        _combatActions = combatActions;
        _combatActionType = combatActionType;
        _actor = actor;

        foreach (CombatAction combatAction in _combatActions)
        {
            GameObject buttonPrefab = Instantiate(_buttonPrefab);
            buttonPrefab.transform.SetParent(_buttonsParent);

            Button combatActionButton = buttonPrefab.GetComponent<Button>();
            combatActionButton.GetComponentInChildren<TextMeshProUGUI>().text = combatAction.Name;
            combatActionButton.onClick.AddListener(() => GetButtonAction(combatAction)());
            combatActionButton.onClick.AddListener(playerInputSystem.UpdateButtonsInteractability);

            _buttons.Add(combatActionButton);
        }
    }

    private Action GetButtonAction(CombatAction combatAction)
    {
        Action doMainButtonAction = combatAction.Identifier switch
        {
            MonsterActionType.Move => () => Debug.Log("Move action"),
            MonsterActionType.Dash => () => Debug.Log("Dash action"),
            MonsterActionType.Disengage => () => combatAction.DoAction(_actor),
            MonsterActionType.Dodge => () => combatAction.DoAction(_actor),
            MonsterActionType.Grapple => () => Debug.Log("Grapple action"),
            MonsterActionType.Hide => () => Debug.Log("Hide action"),
            MonsterActionType.Seek => () => Debug.Log("Seek action"),
            _ => () => Debug.LogWarning("Unknown action")
        };

        Action consumeActionType = _combatActionType switch
        {
            CombatActionType.FreeAction => () => { },
            CombatActionType.MainAction => () => _actor.MainActionAvailable = false,
            CombatActionType.BonusAction => () => _actor.BonusActionAvailable = false,
            _ => () => { }
        };

        Action buttonAction = () => { doMainButtonAction(); consumeActionType(); };

        return doMainButtonAction;
    }
}
