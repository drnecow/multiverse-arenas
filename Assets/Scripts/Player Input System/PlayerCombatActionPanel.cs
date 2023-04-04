using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Constants;
using Project.Utils;

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

            _buttons.Add(combatActionButton);
        }
    }

    private Action GetButtonAction(CombatAction combatAction)
    {
        Action doMainButtonAction = combatAction.Identifier switch
        {
            MonsterActionType.Move => () => StartCoroutine(HandleMoveAction((Move)combatAction, false)),
            MonsterActionType.Dash => () => StartCoroutine(HandleMoveAction((Move)combatAction, true)),
            MonsterActionType.Disengage => () => combatAction.DoAction(_actor, _combatActionType),
            MonsterActionType.Dodge => () => combatAction.DoAction(_actor, _combatActionType),
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

    public override void SetAllButtonsInteractabilityByCondition()
    {
        for (int i = 0; i < _combatActions.Count; i++)
        {
            bool interactable = _combatActions[i].CheckPlayerButtonInteractabilityCondition(_actor, _combatActionType);
            _buttons[i].interactable = interactable;
        }
    }

    private IEnumerator HandleMoveAction(Move moveAction, bool isDash)
    {
        GridMap map = _actor.CombatDependencies.Map;
        MapHighlight highlight = _actor.CombatDependencies.Highlight;

        Coords lastMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());
        bool actionPerformed = false;

        bool isActorSingleCellSized = GridMap.IsSingleCelledSize(_actor.Stats.Size);

        if (isActorSingleCellSized)
        {
            List<Coords> singleCellPath = FindAndHighlightSingleCellMovementPath(lastMouseCoords, map, highlight, isDash);

            while (!actionPerformed)
            {
                Coords currentMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());

                if (currentMouseCoords != lastMouseCoords)
                {
                    singleCellPath = FindAndHighlightSingleCellMovementPath(currentMouseCoords, map, highlight, isDash);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (singleCellPath != null)
                    {
                        moveAction.DoAction(_actor, singleCellPath, _combatActionType);
                        highlight.ClearHighlight();
                        actionPerformed = true;
                    }
                }
                yield return null;
            }
        }
        else
        {
            List<List<Coords>> multipleCellPath = FindAndHighlightMultipleCellMovementPath(lastMouseCoords, map, highlight, isDash);

            while (!actionPerformed)
            {
                Coords currentMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());

                if (currentMouseCoords != lastMouseCoords)
                {
                    multipleCellPath = FindAndHighlightMultipleCellMovementPath(currentMouseCoords, map, highlight, isDash);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (multipleCellPath != null)
                    {
                        moveAction.DoAction(_actor, multipleCellPath, _combatActionType);
                        highlight.ClearHighlight();
                        actionPerformed = true;
                    }
                }
                yield return null;
            }
        }
    }
    private List<Coords> FindAndHighlightSingleCellMovementPath(Coords targetCoords, GridMap map, MapHighlight highlight, bool isDash)
    {
        List<Coords> path = map.FindPathForSingleCellEntity(_actor.CurrentCoordsOriginCell, targetCoords);

        if (path != null)
        {
            highlight.ClearHighlight();
            highlight.HighlightCells(_actor.CurrentCoords, Color.grey);

            int remainingSpeedCells = isDash ? _actor.Stats.Speed.GetSpeedCells(Speed.Walk) : _actor.RemainingSpeed.GetSpeedCells(Speed.Walk);

            List<Coords> availablePathCells = path.GetRange(0, Mathf.Min(remainingSpeedCells, path.Count));
            highlight.HighlightCells(availablePathCells, Color.green);

            if (remainingSpeedCells < path.Count)
            {
                List<Coords> unavailablePathCells = path.GetRange(remainingSpeedCells, path.Count - remainingSpeedCells);
                highlight.HighlightCells(unavailablePathCells, Color.red);
            }

            return availablePathCells;
        }

        return null;
    }
    // TODO: implement
    private List<List<Coords>> FindAndHighlightMultipleCellMovementPath(Coords targetCoords, GridMap map, MapHighlight highlight, bool isDash)
    {
        return null;
    }
}
