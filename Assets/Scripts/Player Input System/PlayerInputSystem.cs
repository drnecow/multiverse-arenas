using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Project.Constants;
using Project.Utils;

public class PlayerInputSystem : MonoBehaviour
{
    private Monster _actor;
    private Move _moveAction;
    private bool _subscribedToMove = false;
    private bool _movementHighlightInterrupted = false;

    [SerializeField] private PlayerCombatActionPanel _mainActionsPanel;
    [SerializeField] private PlayerAttackPanel _attacksPanel;
    [SerializeField] private PlayerCombatActionPanel _bonusActionsPanel;
    [SerializeField] private PlayerCombatActionPanel _freeActionsPanel;

    [SerializeField] private Button _attacksButton;
    [SerializeField] private Button _endTurnButton;

    public event Action OnPlayerEndTurn;


    private void Awake ()
    {
        _freeActionsPanel.SetParentInputSystem(this);
        _attacksPanel.SetParentInputSystem(this);
        _mainActionsPanel.SetParentInputSystem(this);
        _bonusActionsPanel.SetParentInputSystem(this);

        _endTurnButton.onClick.AddListener(EndTurn);

        OnPlayerEndTurn += () => _actor.CombatDependencies.Highlight.ClearHighlight();
        OnPlayerEndTurn += () => _actor.RemoveActiveCondition(Condition.Disengaging);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            _movementHighlightInterrupted = !_movementHighlightInterrupted;
            
            if (_movementHighlightInterrupted)
                _actor.CombatDependencies.Highlight.ClearHighlight();
        }
    }
    public void FillActionPanels(Monster actor)
    {
        _actor = actor;
        _moveAction = _actor.CombatActions.FindFreeActionOfType(MonsterActionType.Move) as Move;

        if (!_subscribedToMove)
        {
            _moveAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) InterruptCurrentCoroutines(); };
            _moveAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };

            _subscribedToMove = true;
        }

        _movementHighlightInterrupted = true;

        List<CombatAction> freeActions = _actor.CombatActions.FreeActions.Where(freeAction => freeAction.Identifier != MonsterActionType.Move).ToList();
        MeleeAttackIntDictionary meleeAttacks = _actor.CombatActions.MeleeAttacks;
        RangedAttackIntDictionary rangedAttacks = _actor.CombatActions.RangedAttacks;
        List<CombatAction> mainActions = _actor.CombatActions.MainActions;
        List<CombatAction> bonusActions = _actor.CombatActions.BonusActions;

        if (freeActions.Count > 0)
        {
            _freeActionsPanel.gameObject.SetActive(true);
            _freeActionsPanel.CreateButtons(freeActions, CombatActionType.FreeAction, _actor);
            _freeActionsPanel.SetAllButtonsInteractabilityByCondition();

            foreach (CombatAction freeAction in freeActions)
            {
                freeAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
            }
        }
        else
            _freeActionsPanel.gameObject.SetActive(false);

        Dictionary<Attack, int> combinedAttacks = new Dictionary<Attack, int>();
        foreach (KeyValuePair<MeleeAttack, int> meleeAttackIntPair in meleeAttacks)
            combinedAttacks.Add(meleeAttackIntPair.Key, meleeAttackIntPair.Value);
        foreach (KeyValuePair<RangedAttack, int> rangedAttackIntPair in rangedAttacks)
            combinedAttacks.Add(rangedAttackIntPair.Key, rangedAttackIntPair.Value);

        _attacksPanel.gameObject.SetActive(false);
        if (combinedAttacks.Count > 0)
        {
            _attacksButton.interactable = true;
            _attacksPanel.CreateButtons(combinedAttacks, actor);
            _attacksPanel.SetAllButtonsInteractabilityByCondition();

            foreach (Attack attack in combinedAttacks.Keys)
            {
                attack.OnAttackAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
            }
        }
        else
            _attacksButton.interactable = false;

        if (mainActions.Count > 0)
        {
            _mainActionsPanel.gameObject.SetActive(true);
            _mainActionsPanel.CreateButtons(mainActions, CombatActionType.MainAction, actor);
            _mainActionsPanel.SetAllButtonsInteractabilityByCondition();

            foreach (CombatAction mainAction in mainActions)
            {
                mainAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
            }
        }
        else
            _mainActionsPanel.gameObject.SetActive(false);

        if (bonusActions.Count > 0)
        {
            _bonusActionsPanel.gameObject.SetActive(true);
            _bonusActionsPanel.CreateButtons(bonusActions, CombatActionType.BonusAction, actor);
            _bonusActionsPanel.SetAllButtonsInteractabilityByCondition();

            foreach (CombatAction bonusAction in bonusActions)
            {
                bonusAction.OnActionAnimationStartedPlaying += (monster, animationDuration) => { if (gameObject.activeSelf) StartCoroutine(SuspendButtonsAvailabilityFor(animationDuration)); };
            }
        }
        else
            _bonusActionsPanel.gameObject.SetActive(false);

        StartCoroutine(HandleMove());
    }
    public void ClearActionPanels()
    {
        List<PlayerActionPanel> panels = GetAllActiveActionPanels();

        foreach (PlayerActionPanel panel in panels)
            panel.ClearAllButtons();
    }

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
    public void InterruptCurrentCoroutines()
    {
        List<PlayerActionPanel> activePanels = GetAllActiveActionPanels();

        foreach (PlayerActionPanel panel in activePanels)
            panel.StopAllCoroutines();

        _movementHighlightInterrupted = true;
    }
    private List<PlayerActionPanel> GetAllActiveActionPanels()
    {
        List<PlayerActionPanel> allPanels = new List<PlayerActionPanel>();

        if (_freeActionsPanel.gameObject.activeSelf)
            allPanels.Add(_freeActionsPanel);
        if (_attacksButton.interactable)
            allPanels.Add(_attacksPanel);
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

    private IEnumerator HandleMove()
    {
        GridMap map = _actor.CombatDependencies.Map;
        MapHighlight highlight = _actor.CombatDependencies.Highlight;

        Coords lastMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());

        List<Coords> path = FindAndHighlightMovementPath(lastMouseCoords, map, highlight);

        while (true)
        {
            Coords currentMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());

            if (currentMouseCoords != lastMouseCoords)
            {
                path = FindAndHighlightMovementPath(currentMouseCoords, map, highlight);
            }
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (path != null)
                {
                    if (path.Count <= _actor.RemainingSpeed.GetSpeedCells(Speed.Walk))
                    {
                        _moveAction.DoAction(_actor, path, CombatActionType.FreeAction);
                        highlight.ClearHighlight();

                        if (_actor.RemainingSpeed.GetSpeedCells(Speed.Walk) == 0)
                            yield break;
                    }
                }
            }
            yield return null;
        }
    }
    private List<Coords> FindAndHighlightMovementPath(Coords targetCoords, GridMap map, MapHighlight highlight)
    {
        List<Coords> path = map.FindPathForMonster(_actor, targetCoords, targetMonster: null);

        if (path != null)
        {
            highlight.ClearHighlight();
            highlight.HighlightCells(_actor.CurrentCoords, Color.grey);

            int remainingSpeedCells = _actor.RemainingSpeed.GetSpeedCells(Speed.Walk);

            List<Coords> availablePathCells = path.GetRange(0, Mathf.Min(remainingSpeedCells, path.Count));
            foreach (Coords availableCell in availablePathCells)
            {
                List<Coords> cellSpace = map.GetListOfMonsterCoords(availableCell, _actor.Stats.Size);
                highlight.HighlightCells(cellSpace, Color.green);
            }

            if (remainingSpeedCells < path.Count)
            {
                List<Coords> unavailablePathCells = path.GetRange(remainingSpeedCells, path.Count - remainingSpeedCells);
                foreach (Coords unavailableCell in unavailablePathCells)
                {
                    List<Coords> cellSpace = map.GetListOfMonsterCoords(unavailableCell, _actor.Stats.Size);
                    highlight.HighlightCells(cellSpace, Color.red);
                }
            }

            highlight.CreateMapText(path[path.Count - 1], $"{path.Count * 5} ft.");

            return path;
        }

        return null;
    }
}
