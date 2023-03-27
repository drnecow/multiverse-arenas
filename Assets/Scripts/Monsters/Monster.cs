using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;
using Project.Dice;

public class Monster : MonoBehaviour
{
    [SerializeField] private bool _isPlayerControlled;
    public bool IsPlayerControlled { get => _isPlayerControlled; set => _isPlayerControlled = value; }

    [field: SerializeField] public CombatDependencies CombatDependencies { get; private set; }

    public Animator Animator { get; private set; }

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public MonsterStats Stats { get; private set; }
    [SerializeField] private int _numberOfAttacks;

    [field: SerializeField] public MonsterCombatActions CombatActions { get; private set; }

    public HashSet<Condition> ActiveConditions { get; private set; }
    public void AddActiveCondition(Condition condition)
    {
        ActiveConditions.Add(condition);
    }
    public void RemoveActiveCondition(Condition condition)
    {
        ActiveConditions.Remove(condition);
    }
    public Dictionary<Condition, Monster> ConditionSourceMonsters { get; private set; }
    public void AddConditionSourceMonster(Condition condition, Monster monster)
    {
        if (!ConditionSourceMonsters.ContainsKey(condition))
            ConditionSourceMonsters[condition] = monster;
        else
            Debug.LogWarning($"Monster {this} already has {condition} condition source monster: {ConditionSourceMonsters[condition]}");
    }
    public void RemoveConditionSourceMonster(Condition condition)
    {
        if (ConditionSourceMonsters.ContainsKey(condition))
            ConditionSourceMonsters.Remove(condition);
        else
            Debug.Log($"Monster {this} has no condition source of type {condition}");
    }

    public Coords CurrentCoordsOriginCell //returns top left square of monster's position; the rest of the squares are deduced through monster's Size
    {
        get
        {
            float cellSize = ConstantValues.MAP_CELL_SIZE;

            return Stats.Size switch
            {
                Size.Tiny => CombatDependencies.Map.WorldPositionToXY(transform.position),
                Size.Small => CombatDependencies.Map.WorldPositionToXY(transform.position),
                Size.Medium => CombatDependencies.Map.WorldPositionToXY(transform.position),
                Size.Large => CombatDependencies.Map.WorldPositionToXY(new Vector3(transform.position.x - cellSize/2, transform.position.y + cellSize/2)),
                Size.Huge => CombatDependencies.Map.WorldPositionToXY(new Vector3(transform.position.x - cellSize, transform.position.y + cellSize)),
                Size.Gargantuan => CombatDependencies.Map.WorldPositionToXY(new Vector3(transform.position.x - (cellSize + cellSize/2), transform.position.y + (cellSize + cellSize/2))),
                _ => CombatDependencies.Map.WorldPositionToXY(transform.position)
            };
        }
    }
    public List<Coords> CurrentCoords
    {
        get => CombatDependencies.Map.GetListOfMonsterCoords(CurrentCoordsOriginCell, Stats.Size);
    }


    private bool _mainActionAvailable = true;
    public bool MainActionAvailable { get => _mainActionAvailable; set => _mainActionAvailable = value; }
    private bool _bonusActionAvailable = true;
    public bool BonusActionAvailable { get => _bonusActionAvailable; set => _bonusActionAvailable = value; }
    private bool _reactionAvailable = true;
    public bool ReactionAvailable { get => _reactionAvailable; set => _reactionAvailable = value; }

    [field: SerializeField] public List<Monster> VisibleTargets { get; private set; }
    

    private void Awake()
    {
        Animator = gameObject.GetComponent<Animator>();
        ActiveConditions = new HashSet<Condition>();
    }

    public bool RollSkillContest(Skill thisMonsterSkill, RollMode thisMonsterRollMode, Monster enemy, Skill enemySkill, RollMode enemyRollMode)
    {
        int thisMonsterSkillModifier = Stats.GetSkillModifier(thisMonsterSkill);
        int enemySkillModifier = enemy.Stats.GetSkillModifier(enemySkill);

        int thisMonsterRoll = Dice.RollD20(rollMode: thisMonsterRollMode) + thisMonsterSkillModifier;
        int enemyRoll = Dice.RollD20(rollMode: enemyRollMode) + enemySkillModifier;

        if (thisMonsterRoll >= enemyRoll)
            return true;
        else
            return false;
    }
    public int TakeDamage(int damagePoints, DamageType damageType)
    {
        int totalDamageTaken = damagePoints;

        if (Stats.DamageImmunities.Contains(damageType))
        {
            Debug.Log($"{Name} is immune to {damageType} damage, so no damage to it");
            totalDamageTaken = 0;
        }
        else if (Stats.DamageResistances.Contains(damageType))
        {
            totalDamageTaken = damagePoints / 2;
            Debug.Log($"{Name} is resistant to {damageType} damage, so half damage to it: {totalDamageTaken}");
            Stats.CurrentHP -= totalDamageTaken;
        }
        else if (Stats.DamageVulnerabilities.Contains(damageType))
        {
            totalDamageTaken = damagePoints * 2;
            Debug.Log($"{Name} is vulnerable to {damageType} damage, so double damage to it: {totalDamageTaken}");
            Stats.CurrentHP -= totalDamageTaken;
        }
        else
        {
            Debug.Log($"{Name} takes {damagePoints} points of {damageType} damage");
            Stats.CurrentHP -= damagePoints;
        }

        return totalDamageTaken;
    }
    public void EscapeGrapple(bool useAthletics)
    {
        if (!ConditionSourceMonsters.ContainsKey(Condition.Grappled))
            Debug.Log($"Cannot escape grapple since {this} is not grappled");
        
        Debug.Log($"{Name} is trying to escape grapple");

        Monster grappler = ConditionSourceMonsters[Condition.Grappled];

        Skill thisMonsterSkill = useAthletics ? Skill.Athletics : Skill.Acrobatics;
        RollMode thisMonsterRollMode = ResolveAdvantageAndDisadvantageToSkillCheck(thisMonsterSkill);
        RollMode enemyRollMode = grappler.ResolveAdvantageAndDisadvantageToSkillCheck(Skill.Athletics);

        bool success = RollSkillContest(thisMonsterSkill, thisMonsterRollMode, grappler, Skill.Athletics, enemyRollMode);

        if (success)
        {
            RemoveActiveCondition(Condition.Grappled);
            RemoveConditionSourceMonster(Condition.Grappled);
            Debug.Log($"{Name} successfully escaped grapple from {grappler.Name}");
        }
        else
            Debug.Log($"{Name} could not escape grapple from {grappler.Name}");
    }

    //TODO: implement this method
    public RollMode ResolveAdvantageAndDisadvantageToMeleeAttack(Monster target)
    {
        return RollMode.Normal;
    }
    //TODO: implement this method
    public RollMode ResolveAdvantageAndDisadvantageToRangedAttack(Monster target, int feetToTarget)
    {
        return RollMode.Normal;
    }
    //TODO: implement this method
    public RollMode ResolveAdvantageAndDisadvantageToSkillCheck(Skill skill)
    {
        return RollMode.Normal;
    }
}
