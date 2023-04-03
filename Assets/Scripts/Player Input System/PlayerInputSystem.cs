using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.Constants;

public class PlayerInputSystem : MonoBehaviour
{
    private Monster _actor;

    [SerializeField] private PlayerCombatActionPanel _freeActionsPanel;
    [SerializeField] private PlayerMeleeAttackPanel _meleeAttackPanel;
    [SerializeField] private PlayerRangedAttackPanel _rangedAttackPanel;
    [SerializeField] private PlayerCombatActionPanel _mainActionsPanel;
    [SerializeField] private PlayerCombatActionPanel _bonusActionsPanel;

    [SerializeField] private Button _endTurnButton;

    public event Action OnPlayerEndTurn;


    private void Awake()
    {
        OnPlayerEndTurn += () => _actor.IsDisengaging = false;

        _endTurnButton.onClick.AddListener(EndTurn);
    }
    public void FillActionPanels(Monster actor)
    {
        _actor = actor;

        List<CombatAction> freeActions = _actor.CombatActions.FreeActions;
        MeleeAttackIntDictionary meleeAttacks = _actor.CombatActions.MeleeAttacks;
        RangedAttackIntDictionary rangedAttacks = _actor.CombatActions.RangedAttacks;
        List<CombatAction> mainActions = _actor.CombatActions.MainActions;
        List<CombatAction> bonusActions = _actor.CombatActions.BonusActions;

        if (freeActions.Count > 0)
        {
            _freeActionsPanel.gameObject.SetActive(true);
            _freeActionsPanel.CreateButtons(freeActions, CombatActionType.FreeAction, _actor, this);
        }
        else
            _freeActionsPanel.gameObject.SetActive(false);

        if (meleeAttacks.Count > 0)
        {
            _meleeAttackPanel.gameObject.SetActive(true);
            _meleeAttackPanel.CreateButtons(meleeAttacks, actor, this);
        }
        else
            _meleeAttackPanel.gameObject.SetActive(false);

        if (rangedAttacks.Count > 0)
        {
            _rangedAttackPanel.gameObject.SetActive(true);
            _rangedAttackPanel.CreateButtons(rangedAttacks, actor, this);
        }
        else
            _rangedAttackPanel.gameObject.SetActive(false);

        if (mainActions.Count > 0)
        {
            _mainActionsPanel.gameObject.SetActive(true);
            _mainActionsPanel.CreateButtons(mainActions, CombatActionType.MainAction, actor, this);
        }
        else
            _mainActionsPanel.gameObject.SetActive(false);

        if (bonusActions.Count > 0)
        {
            _bonusActionsPanel.gameObject.SetActive(true);
            _bonusActionsPanel.CreateButtons(bonusActions, CombatActionType.BonusAction, actor, this);
        }
        else
            _bonusActionsPanel.gameObject.SetActive(false);
    }
    public void ClearActionPanels()
    {
        _freeActionsPanel.DestroyAllButtons();
        _meleeAttackPanel.DestroyAllButtons();
        _rangedAttackPanel.DestroyAllButtons();
        _mainActionsPanel.DestroyAllButtons();
        _bonusActionsPanel.DestroyAllButtons();
    }

    public void UpdateButtonsInteractability()
    {
        Debug.Log("Updating interactability of all buttons");
    }
    private void EndTurn()
    {
        ClearActionPanels();
        OnPlayerEndTurn?.Invoke();
    }
}
