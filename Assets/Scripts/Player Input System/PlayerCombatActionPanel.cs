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

            Image combatActionImage = _actionSlots[i].GetComponent<Image>();
            combatActionImage.sprite = _visuals.GetSpriteForMonsterAction(combatAction.Identifier);
            combatActionImage.color = Color.white;

            TooltipTarget combatActionTooltipTarget = _actionSlots[i].GetComponent<TooltipTarget>();
            combatActionTooltipTarget.SetText(_descriptions.GetMonsterActionDescription(combatAction.Identifier));
            combatActionTooltipTarget.Enabled = true;

            Button combatActionButton = _actionSlots[i].GetComponent<Button>();
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

        List<Coords> path = FindAndHighlightMovementPath(lastMouseCoords, map, highlight, isDash);

        while (true)
        {
            Coords currentMouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());

            if (currentMouseCoords != lastMouseCoords)
            {
                path = FindAndHighlightMovementPath(currentMouseCoords, map, highlight, isDash);
            }
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (path != null)
                {
                   if (isDash && path.Count <= _actor.Stats.Speed.GetSpeedCells(Speed.Walk) ||
                        !isDash && path.Count <= _actor.RemainingSpeed.GetSpeedCells(Speed.Walk))
                   {
                       moveAction.DoAction(_actor, path, _combatActionType);
                       highlight.ClearHighlight();
                       yield break;
                   }
                }
            }
            yield return null;
        }
    }
    private List<Coords> FindAndHighlightMovementPath(Coords targetCoords, GridMap map, MapHighlight highlight, bool isDash)
    {
        List<Coords> path = map.FindPathForMonster(_actor, targetCoords, targetMonster:null);

        if (path != null)
        {
            highlight.ClearHighlight();
            highlight.HighlightCells(_actor.CurrentCoords, Color.grey);

            int remainingSpeedCells = isDash ? _actor.Stats.Speed.GetSpeedCells(Speed.Walk) : _actor.RemainingSpeed.GetSpeedCells(Speed.Walk);

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
