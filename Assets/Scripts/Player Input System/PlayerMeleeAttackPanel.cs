using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMeleeAttackPanel : PlayerActionPanel
{
    private MeleeAttackIntDictionary _meleeAttacks;


    public void CreateButtons(MeleeAttackIntDictionary meleeAttacks, Monster actor, PlayerInputSystem playerInputSystem)
    {
        _actor = actor;
        _meleeAttacks = meleeAttacks;

        foreach (MeleeAttack meleeAttack in _meleeAttacks.Keys)
        {
            GameObject buttonPrefab = Instantiate(_buttonPrefab);
            buttonPrefab.transform.SetParent(_buttonsParent);

            Button combatActionButton = buttonPrefab.GetComponent<Button>();
            combatActionButton.GetComponentInChildren<TextMeshProUGUI>().text = meleeAttack.Name;
            //combatActionButton.onClick.AddListener();
            combatActionButton.onClick.AddListener(playerInputSystem.UpdateButtonsInteractability);

            _buttons.Add(combatActionButton);
        }
    }
}
