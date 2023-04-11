using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Project.Constants;
using Project.Utils;

public class PlayerAttackPanel : PlayerActionPanel
{
    private List<Attack> _buttonAttacks;


    public void CreateButtons(Dictionary<Attack, int> attacks, Monster actor)
    {
        _buttons = new List<Button>();

        _actor = actor;
        _buttonAttacks = attacks.Keys.ToList();

        for (int i = 0; i < _buttonAttacks.Count; i++)
        {
            Attack attack = _buttonAttacks[i];

            Image attackImage = _actionSlots[i].GetComponent<Image>();
            attackImage.sprite = _visuals.GetSpriteForAttack(attack.Identifier);
            attackImage.color = Color.white;

            TooltipTarget attackTooltipTarget = _actionSlots[i].GetComponent<TooltipTarget>();
            attackTooltipTarget.SetText(_descriptions.GetAttackDescription(attack.Identifier));
            attackTooltipTarget.Enabled = true;

            Button attackButton = _actionSlots[i].GetComponent<Button>();
            attackButton.onClick.AddListener(() => { _actor.CombatDependencies.Highlight.ClearHighlight(); _parentInputSystem.InterruptCurrentCoroutines(); });

            if (attack is MeleeAttack)
                attackButton.onClick.AddListener(() => StartCoroutine(HighlightPossibleTargetsOfMeleeAttack(attack as MeleeAttack)));
            else if (attack is RangedAttack)
                attackButton.onClick.AddListener(() => StartCoroutine(HighlightPossibleTargetsOfRangedAttack(attack as RangedAttack)));

            attackButton.enabled = true;
            _buttons.Add(attackButton);
        }
    }
    public override void SetAllButtonsInteractabilityByCondition()
    {
        GridMap map = _actor.CombatDependencies.Map;

        for (int i = 0; i < _buttons.Count; i++)
        {
            Attack attack = _buttonAttacks[i];
            HashSet<Monster> monstersInAttackRadius = new HashSet<Monster>();

            if (attack is MeleeAttack)
                monstersInAttackRadius = map.FindMonstersInRadius(_actor.CurrentCoordsOriginCell, _actor.Stats.Size, (attack as MeleeAttack).Reach);
            else if (attack is RangedAttack)
                monstersInAttackRadius = map.FindMonstersInRadius(_actor.CurrentCoordsOriginCell, _actor.Stats.Size, (attack as RangedAttack).DisadvantageRange);

            HashSet<Monster> possibleAttackTargets = new HashSet<Monster>();

            foreach (Monster monster in monstersInAttackRadius)
                if (!monster.IsPlayerControlled && _actor.VisibleTargets.Contains(monster))
                    possibleAttackTargets.Add(monster);

            if (_actor.RemainingAttacks[attack] > 0 && possibleAttackTargets.Count > 0)
                _buttons[i].interactable = true;
            else
                _buttons[i].interactable = false;
        }

        // If main action was used but attack wasn't, forbid attacks; however, if main action was used and attack action was used as well, attacks can be continued up to total maximum
        if (_actor.RemainingTotalAttacks == 0 || (!_actor.MainActionAvailable && _actor.AttackMainActionAvailable))
            foreach (Button button in _buttons)
                button.interactable = false;
    }
    private IEnumerator HighlightPossibleTargetsOfMeleeAttack(MeleeAttack meleeAttack)
    {
        GridMap map = _actor.CombatDependencies.Map;
        MapHighlight highlight = _actor.CombatDependencies.Highlight;

        HashSet<Monster> monstersInReach = map.FindMonstersInRadius(_actor.CurrentCoordsOriginCell, _actor.Stats.Size, meleeAttack.Reach);
        HashSet<Monster> possibleAttackTargets = new HashSet<Monster>();

        highlight.HighlightCells(_actor.CurrentCoords, Color.grey);
        foreach (Monster monster in monstersInReach)
            if (!monster.IsPlayerControlled && _actor.VisibleTargets.Contains(monster))
            {
                possibleAttackTargets.Add(monster);
                highlight.HighlightCells(monster.CurrentCoords, Color.green);
            }

        if (_actor.Stats.SpecialAbilities.Contains(SpecialAbility.PackTactics)) {
            
            foreach (Monster target in possibleAttackTargets)
            {
                if (map.FindMonstersInRadius(target.CurrentCoordsOriginCell, target.Stats.Size, 5).Any(neighbour => neighbour.IsPlayerControlled && neighbour != _actor))
                    HighlightPackTactics(target);
            }
        }

        while (true)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Coords mouseCoords = map.WorldPositionToXY(Utils.GetMouseWorldPosition());
                Monster targetMonster = map.GetGridObjectAtCoords(mouseCoords).Monster;

                if (possibleAttackTargets.Contains(targetMonster))
                {
                    meleeAttack.MakeMeleeAttack(_actor, targetMonster);
                    highlight.ClearHighlight();
                    yield break;
                }  
            }

            yield return null;
        }
    }
    private IEnumerator HighlightPossibleTargetsOfRangedAttack(RangedAttack rangedAttack)
    {
        yield break;
    }

    private void HighlightPackTactics(Monster target)
    {
        _actor.CombatDependencies.Highlight.CreateMonsterStatusIcon(target, _visuals.GetSpriteForSpecialAbility(SpecialAbility.PackTactics));
    }
}
