using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;
using Project.Dice;
using Project.DictionaryStructs;

public class Monster : MonoBehaviour
{
    [SerializeField] private bool _isPlayerControlled;
    public bool IsPlayerControlled { get => _isPlayerControlled; set { _isPlayerControlled = value; OnMonsterAllegianceChanged?.Invoke(_isPlayerControlled); } }

    public CombatDependencies CombatDependencies { get; private set; }
    public void SetCombatDependencies(CombatDependencies combatDependencies)
    {
        CombatDependencies = combatDependencies;
    }

    public Animator Animator { get; private set; }
    public MonsterAnimator MonsterAnimator { get; private set; }

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public MonsterStats Stats { get; private set; }
    public void SetStats(MonsterStats stats)
    {
        Stats = stats;
    }
    [SerializeField] private int _numberOfAttacks;
    public int NumberOfAttacks { get => _numberOfAttacks; }

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
    private bool _attackMainActionAvailable = true;
    public bool AttackMainActionAvailable { get => _attackMainActionAvailable; set => _attackMainActionAvailable = value; }
    private bool _bonusActionAvailable = true;
    public bool BonusActionAvailable { get => _bonusActionAvailable; set => _bonusActionAvailable = value; }
    private bool _reactionAvailable = true;
    public bool ReactionAvailable { get => _reactionAvailable; set => _reactionAvailable = value; }

    private bool _isDisengaging = false;
    public bool IsDisengaging { get => _isDisengaging; set => _isDisengaging = value; }
    private bool _isDodging = false;
    public bool IsDodging { get => _isDodging; set => _isDodging = value; }
    private bool _isHiding = false;
    public bool IsHiding { get => _isHiding; set => _isHiding = value; }
    private int _stealthRoll = -1000;
    public int StealthRoll { get => _stealthRoll; set => _stealthRoll = value; }
    
    private int _remainingTotalAttacks;
    public int RemainingTotalAttacks { get => _remainingTotalAttacks; set => _remainingTotalAttacks = value; }
    public Dictionary<Attack, int> RemainingAttacks { get; private set; }
    public SpeedValues RemainingSpeed { get; private set; }
    
    public HashSet<Monster> VisibleTargets { get; private set; }
    

    public event Action<bool> OnMonsterAllegianceChanged;

    public event Action<int> OnMonsterHPChanged;
    public event Action<Monster> OnMonsterHPReducedToZero;


    private void Awake()
    {
        Animator = gameObject.GetComponent<Animator>();
        MonsterAnimator = gameObject.GetComponent<MonsterAnimator>();
        ActiveConditions = new HashSet<Condition>();
        VisibleTargets = new HashSet<Monster>();

        _remainingTotalAttacks = _numberOfAttacks;
        
        RemainingAttacks = new Dictionary<Attack, int>();
        foreach (KeyValuePair<MeleeAttack, int> meleeAttackIntPair in CombatActions.MeleeAttacks)
            RemainingAttacks.Add(meleeAttackIntPair.Key, meleeAttackIntPair.Value);
        foreach (KeyValuePair<RangedAttack, int> rangedAttackIntPair in CombatActions.RangedAttacks)
            RemainingAttacks.Add(rangedAttackIntPair.Key, rangedAttackIntPair.Value);

        RemainingSpeed = new SpeedValues(Stats.Speed);
    }

    public int RollInitiative()
    {
        int initiativeRoll = Dice.RollD20() + Stats.Abilities.GetAbilityModifier(Ability.Dexterity);
        CombatDependencies.Instance.EventsLogger.LogLocalInfo(this, $"Initiative: {initiativeRoll}");
        return initiativeRoll;
    }
    public int MakeSkillCheck(Skill skill)
    {
        RollMode skillCheckRollMode = ResolveAdvantageAndDisadvantageToSkillCheck(skill);
        int skillModifier = Stats.GetSkillModifier(skill);

        int skillRoll = Dice.RollD20(rollMode: skillCheckRollMode) + skillModifier;
        return skillRoll;
    }
    public bool MakeSkillCheckAgainstDC(Skill skill, int dc)
    {
        int skillRoll = MakeSkillCheck(skill);
        return skillRoll >= dc;
    }
    public bool RollSkillContest(Skill thisMonsterSkill, Monster enemy, Skill enemySkill)
    {
        int thisMonsterRoll = MakeSkillCheck(thisMonsterSkill);
        int enemyRoll = enemy.MakeSkillCheck(enemySkill);

        return (thisMonsterRoll >= enemyRoll);
    }
    public int TakeDamage(int damagePoints, DamageType damageType)
    {
        Debug.Log($"Current HP: {Stats.CurrentHP}");
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

        if (damagePoints > 0)
            OnMonsterHPChanged?.Invoke(Stats.CurrentHP);

        if (Stats.CurrentHP == 0)
            OnMonsterHPReducedToZero?.Invoke(this);
        Debug.Log($"Current HP: {Stats.CurrentHP}");

        return totalDamageTaken;
    }
    public void EscapeGrapple(bool useAthletics)
    {
        if (!ConditionSourceMonsters.ContainsKey(Condition.Grappled))
            Debug.Log($"Cannot escape grapple since {this} is not grappled");
        
        Debug.Log($"{Name} is trying to escape grapple");

        Monster grappler = ConditionSourceMonsters[Condition.Grappled];

        Skill thisMonsterSkill = useAthletics ? Skill.Athletics : Skill.Acrobatics;

        bool success = RollSkillContest(thisMonsterSkill, grappler, Skill.Athletics);

        if (success)
        {
            RemoveActiveCondition(Condition.Grappled);
            RemoveConditionSourceMonster(Condition.Grappled);
            Debug.Log($"{Name} successfully escaped grapple from {grappler.Name}");
        }
        else
            Debug.Log($"{Name} could not escape grapple from {grappler.Name}");
    }

    //TODO: implement this method; include pack tactics
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
