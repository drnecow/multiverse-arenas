using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Project.Constants;
using Project.Utils;

public class PlayerCombatActionPanel : PlayerActionPanel
{
    private List<CombatAction> _combatActions;
    private CombatActionType _combatActionType;


    public void CreateButtons(List<CombatAction> combatActions, CombatActionType combatActionType, Monster actor)
    {
        _buttons = new List<Button>();

        _combatActions = combatActions;
        _combatActionType = combatActionType;
        _actor = actor;

        for (int i = 0; i < _combatActions.Count; i++)
        {
            CombatAction combatAction = _combatActions[i];

            Button combatActionButton = _actionSlots[i].GetComponent<Button>();
            _actionSlots[i].GetComponentInChildren<TextMeshProUGUI>().text = combatAction.Name;
            combatActionButton.onClick.AddListener(() => { _actor.CombatDependencies.Highlight.ClearHighlight(); _parentInputSystem.InterruptCurrentCoroutines(); });
            combatActionButton.onClick.AddListener(() => GetButtonAction(combatAction)());

            combatActionButton.enabled = true;
            _buttons.Add(combatActionButton);
        }
    }

    private Action GetButtonAction(CombatAction combatAction)
    {
        MapHighlight highlight = _actor.CombatDependencies.Highlight;

        Action doMainButtonAction = combatAction.Identifier switch
        {
            MonsterActionType.Dash => () => StartCoroutine(HandleMoveAction((Move)combatAction, true)),
            MonsterActionType.Disengage => () => combatAction.DoAction(_actor, _combatActionType),
            MonsterActionType.Dodge => () => combatAction.DoAction(_actor, _combatActionType),
            MonsterActionType.Grapple => () => Debug.Log("Grapple action"),
            MonsterActionType.Hide => () => combatAction.DoAction(_actor, _combatActionType),
            MonsterActionType.Seek => () => combatAction.DoAction(_actor, _combatActionType),
            _ => () => Debug.LogWarning("Unknown action")
        };

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

        bool isActorSingleCellSized = GridMap.IsSingleCelledSize(_actor.Stats.Size);

        if (isActorSingleCellSized)
        {
            List<Coords> singleCellPath = FindAndHighlightSingleCellMovementPath(lastMouseCoords, map, highlight, isDash);

            while (true)
            {
                Coords currentMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());

                if (currentMouseCoords != lastMouseCoords)
                {
                    singleCellPath = FindAndHighlightSingleCellMovementPath(currentMouseCoords, map, highlight, isDash);
                }
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (singleCellPath != null)
                    {
                        if (isDash && singleCellPath.Count <= _actor.Stats.Speed.GetSpeedCells(Speed.Walk) || !isDash && singleCellPath.Count <= _actor.RemainingSpeed.GetSpeedCells(Speed.Walk))
                        {
                            moveAction.DoAction(_actor, singleCellPath, _combatActionType);
                            highlight.ClearHighlight();
                            yield break;
                        }
                    }
                }

                yield return null;
            }
        }
        else
        {
            List<List<Coords>> multipleCellPath = FindAndHighlightMultipleCellMovementPath(lastMouseCoords, map, highlight, isDash);

            while (true)
            {
                Coords currentMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());

                if (currentMouseCoords != lastMouseCoords)
                {
                    multipleCellPath = FindAndHighlightMultipleCellMovementPath(currentMouseCoords, map, highlight, isDash);
                }
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (multipleCellPath != null)
                    {
                        if (isDash && multipleCellPath.Count <= _actor.Stats.Speed.GetSpeedCells(Speed.Walk) || !isDash && multipleCellPath.Count <= _actor.RemainingSpeed.GetSpeedCells(Speed.Walk))
                        {
                            moveAction.DoAction(_actor, multipleCellPath, _combatActionType);
                            highlight.ClearHighlight();
                            yield break;
                        }
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

            highlight.CreateMapText(path[path.Count - 1], $"{path.Count * 5} ft.");

            return path;
        }

        return null;
    }
    // TODO: implement
    private List<List<Coords>> FindAndHighlightMultipleCellMovementPath(Coords targetCoords, GridMap map, MapHighlight highlight, bool isDash)
    {
        return null;
    }
}
