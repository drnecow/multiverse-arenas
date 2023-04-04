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
        Debug.Log($"Actor (PlayerInputSystem): {_actor}");

        List<CombatAction> freeActions = _actor.CombatActions.FreeActions;
        MeleeAttackIntDictionary meleeAttacks = _actor.CombatActions.MeleeAttacks;
        RangedAttackIntDictionary rangedAttacks = _actor.CombatActions.RangedAttacks;
        List<CombatAction> mainActions = _actor.CombatActions.MainActions;
        List<CombatAction> bonusActions = _actor.CombatActions.BonusActions;

        if (freeActions.Count > 0)
        {
            _freeActionsPanel.gameObject.SetActive(true);
            _freeActionsPanel.CreateButtons(freeActions, CombatActionType.FreeAction, _actor, this);
            _freeActionsPanel.SetAllButtonsInteractabilityByCondition();

            foreach (CombatAction freeAction in freeActions)
                freeAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
        }
        else
            _freeActionsPanel.gameObject.SetActive(false);

        if (meleeAttacks.Count > 0)
        {
            _meleeAttackPanel.gameObject.SetActive(true);
            _meleeAttackPanel.CreateButtons(meleeAttacks, actor, this);
            _meleeAttackPanel.SetAllButtonsInteractabilityByCondition();

            foreach (MeleeAttack meleeAttack in meleeAttacks.Keys)
                meleeAttack.OnAttackAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
        }
        else
            _meleeAttackPanel.gameObject.SetActive(false);

        if (rangedAttacks.Count > 0)
        {
            _rangedAttackPanel.gameObject.SetActive(true);
            _rangedAttackPanel.CreateButtons(rangedAttacks, actor, this);
            _rangedAttackPanel.SetAllButtonsInteractabilityByCondition();

            foreach (RangedAttack rangedAttack in rangedAttacks.Keys)
                rangedAttack.OnAttackAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
        }
        else
            _rangedAttackPanel.gameObject.SetActive(false);

        if (mainActions.Count > 0)
        {
            _mainActionsPanel.gameObject.SetActive(true);
            _mainActionsPanel.CreateButtons(mainActions, CombatActionType.MainAction, actor, this);
            _mainActionsPanel.SetAllButtonsInteractabilityByCondition();

            foreach (CombatAction mainAction in mainActions)
                mainAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
        }
        else
            _mainActionsPanel.gameObject.SetActive(false);

        if (bonusActions.Count > 0)
        {
            _bonusActionsPanel.gameObject.SetActive(true);
            _bonusActionsPanel.CreateButtons(bonusActions, CombatActionType.BonusAction, actor, this);
            _bonusActionsPanel.SetAllButtonsInteractabilityByCondition();

            foreach (CombatAction bonusAction in bonusActions)
                bonusAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
        }
        else
            _bonusActionsPanel.gameObject.SetActive(false);
    }
    public void ClearActionPanels()
    {
        List<PlayerActionPanel> panels = GetAllActiveActionPanels();

        foreach (PlayerActionPanel panel in panels)
            panel.DestroyAllButtons();
    }

    /*public void UpdateButtonsInteractability()
    {
        List<PlayerActionPanel> panels = GetAllActionPanels();

        foreach (PlayerActionPanel panel in panels)
            panel.SetAllButtonsInteractabilityByCondition();

        Debug.Log("Updating interactability of all buttons");
    }*/
    private IEnumerator SuspendButtonsAvailabilityFor(float timeInSeconds)
    {
        List<PlayerActionPanel> panels = GetAllActiveActionPanels();

        foreach (PlayerActionPanel panel in panels)
            panel.SetAllButtonsNonInteractable();
        _endTurnButton.interactable = false;

        yield return new WaitForSeconds(timeInSeconds);

        foreach (PlayerActionPanel panel in panels)
            panel.SetAllButtonsInteractabilityByCondition();
        _endTurnButton.interactable = true;
    }
    private List<PlayerActionPanel> GetAllActiveActionPanels()
    {
        List<PlayerActionPanel> allPanels = new List<PlayerActionPanel>();

        if (_freeActionsPanel.gameObject.activeSelf)
            allPanels.Add(_freeActionsPanel);
        if (_meleeAttackPanel.gameObject.activeSelf)
            allPanels.Add(_meleeAttackPanel);
        if (_rangedAttackPanel.gameObject.activeSelf)
            allPanels.Add(_rangedAttackPanel);
        if (_mainActionsPanel.gameObject.activeSelf)
            allPanels.Add(_mainActionsPanel);
        if (_bonusActionsPanel.gameObject.activeSelf)
            allPanels.Add(_bonusActionsPanel);

        return allPanels;
    }
    private void EndTurn()
    {
        ClearActionPanels();
        OnPlayerEndTurn?.Invoke();
    }
}
