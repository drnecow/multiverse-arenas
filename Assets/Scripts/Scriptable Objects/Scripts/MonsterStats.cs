using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;
using Project.DictionaryStructs;
using Project.Dice;


[CreateAssetMenu(fileName = "MonsterStats", menuName = "Scriptable Objects/Monster Stats")]
public class MonsterStats : ScriptableObject
{
    [SerializeField] private string _name = "Unspecified Monster";

    [field: SerializeField] public Abilities Abilities { get; private set; }
    [SerializeField] private int _proficiencyBonus;
    [field: SerializeField] public SkillProficiencies SkillProficiencies { get; private set; }
    [field: SerializeField] public SavingThrowProficiencies SaveProficiencies { get; private set; }

    [SerializeField] private CreatureType _creatureType;
    [SerializeField] private Size _size;
    [SerializeField] private float _challengeRating;

    public int ArmorClass
    {
        get => 10 + _naturalArmorBonus + _wornArmorBonus + _shieldBonus + Mathf.Min(Abilities.GetAbilityModifier(Ability.Dexterity), _maximumDexModifierAllowed) + _coverBonus;
    }
    [SerializeField] private int _naturalArmorBonus;
    private int _wornArmorBonus = 0;
    private int _shieldBonus = 0;
    private int _maximumDexModifierAllowed = 1000;
    private int _coverBonus = 0;

    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
    [SerializeField] private Die _hitDie;
    [SerializeField] private int _numberOfHitDice;

    [field: SerializeField] public SpeedValues Speed { get; private set; }
    [field: SerializeField] public Senses Senses { get; private set; }

    [SerializeField] private List<DamageType> _damageResistances;
    [SerializeField] private List<DamageType> _damageImmunities;
    [SerializeField] private List<DamageType> _damageVulnerabilities;
    [SerializeField] private List<Condition> _conditionImmunities;

    [SerializeField] private List<SpecialAbility> _specialAbilities;


    // DATA MANAGEMENT PROPERTIES AND FUNCTIONS
    public string Name { get => _name; set => _name = value; }

    public int ProficiencyBonus
    {
        get => _proficiencyBonus;
        set
        {
            if (value < ConstantValues.MIN_PROFICIENCY_BONUS)
                _proficiencyBonus = ConstantValues.MIN_PROFICIENCY_BONUS;
            else if (value > ConstantValues.MAX_PROFICIENCY_BONUS)
                _proficiencyBonus = ConstantValues.MAX_PROFICIENCY_BONUS;
            else
                _proficiencyBonus = value;
        }
    }

    public CreatureType CreatureType { get => _creatureType; set => _creatureType = value; }
    public Size Size { get => _size; set => _size = value; }
    public float ChallengeRating
    {
        get => _challengeRating;
        set
        {
            if (value == 0 || value == 0.125 || value == 0.25 || value == 0.5 || (value >= 1 && value == Mathf.Round(value)))
                _challengeRating = value;
            else
                Debug.LogWarning($"Invalid challenge rating value: {value}");
        }
    }

    public int MaxHP
    {
        get { return _maxHP; }
    }
    public int CurrentHP
    {
        get { return _currentHP; }
        set { _currentHP = Mathf.Clamp(value, 0, _maxHP); }
    }
    // This methods both validates new _maxHP value and makes _currentHP fit it if necessary
    public void SetMaxHPProperly(int newValue)
    {
        if (newValue > 0)
        {
            _maxHP = newValue;

            if (_maxHP < _currentHP)
                _currentHP = _maxHP;
        }
        else
            Debug.LogWarning($"Invalid max HP value: {newValue}");
    }
    public void SetMaxHPAndCurrentHP(int newValue)
    {
        if (newValue > 0)
        {
            _maxHP = newValue;
            _currentHP = _maxHP;
        }
        else
            Debug.LogWarning($"Invalid max HP value: {newValue}");
    }
    public Die HitDie { get => _hitDie; set => _hitDie = value; }
    public int NumberOfHitDice
    {
        get => _numberOfHitDice;
        set
        {
            if (value > 0)
                _numberOfHitDice = value;
            else
                Debug.LogWarning($"Invalid number of hit dice value: {value}");
        }
    }


    public List<DamageType> DamageResistances { get => _damageResistances; }
    public void AddDamageResistance(DamageType resistance)
    {
        if (!_damageResistances.Contains(resistance))
        {
            _damageResistances.Add(resistance);
            Debug.Log($"Added damage resistance {resistance}");
        }
        else
            Debug.Log($"This creature already has damage resistance of type {resistance}");
    }
    public void RemoveDamageResistance(DamageType resistance)
    {
        if (_damageResistances.Contains(resistance))
        {
            _damageResistances.Remove(resistance);
            Debug.Log($"Removed damage resistance {resistance}");
        }
        else
            Debug.Log($"This creature doesn't have damage resistance of type {resistance}, so nothing to remove");
    }

    public List<DamageType> DamageImmunities { get => _damageImmunities; }
    public void AddDamageImmunity(DamageType immunity)
    {
        if (!_damageImmunities.Contains(immunity))
        {
            _damageImmunities.Add(immunity);
            Debug.Log($"Added damage immunity {immunity}");
        }
        else
            Debug.Log($"This creature already has damage immunity of type {immunity}");
    }
    public void RemoveDamageImmunity(DamageType immunity)
    {
        if (_damageImmunities.Contains(immunity))
        {
            _damageImmunities.Remove(immunity);
            Debug.Log($"Removed damage immunity {immunity}");
        }
        else
            Debug.Log($"This creature doesn't have damage immunity of type {immunity}, so nothing to remove");
    }

    public List<DamageType> DamageVulnerabilities { get => _damageVulnerabilities; }
    public void AddDamageVulnerability(DamageType vulnerability)
    {
        if (!_damageVulnerabilities.Contains(vulnerability))
        {
            _damageVulnerabilities.Add(vulnerability);
            Debug.Log($"Added damage vulnerability {vulnerability}");
        }
        else
            Debug.Log($"This creature already has damage vulnerability of type {vulnerability}");
    }
    public void RemoveDamageVulnerability(DamageType vulnerability)
    {
        if (_damageVulnerabilities.Contains(vulnerability))
        {
            _damageVulnerabilities.Remove(vulnerability);
            Debug.Log($"Removed damage vulnerability {vulnerability}");
        }
        else
            Debug.Log($"This creature doesn't have damage vulnerability of type {vulnerability}, so nothing to remove");
    }

    public List<Condition> ConditionImmunities { get => _conditionImmunities; }
    public void AddConditionImmunity(Condition immunity)
    {
        if (!_conditionImmunities.Contains(immunity))
        {
            _conditionImmunities.Add(immunity);
            Debug.Log($"Added condition immunity {immunity}");
        }
        else
            Debug.Log($"This creature already has condition immunity of type {immunity}");
    }
    public void RemoveConditionImmunity(Condition immunity)
    {
        if (_conditionImmunities.Contains(immunity))
        {
            _conditionImmunities.Remove(immunity);
            Debug.Log($"Removed condition immunity {immunity}");
        }
        else
            Debug.Log($"This creature doesn't have condition immunity of type {immunity}, so nothing to remove");
    }

    public List<SpecialAbility> SpecialAbilities { get => _specialAbilities; }

    public void AddSpecialAbility(SpecialAbility ability)
    {
        if (!_specialAbilities.Contains(ability))
        {
            _specialAbilities.Add(ability);
            Debug.Log($"Added special ability {ability}");
        }
        else
            Debug.Log($"This creature already has special ability of type {ability}");
    }
    public void RemoveSpecialAbility(SpecialAbility ability)
    {
        if (_specialAbilities.Contains(ability))
        {
            _specialAbilities.Remove(ability);
            Debug.Log($"Removed special ability {ability}");
        }
        else
            Debug.Log($"This creature doesn't have special ability of type {ability}, so nothing to remove");
    }


    // LOGIC FUNCTIONS
    public void RollHitPoints()
    {
        int newHitPoints = Dice.RollDice(_hitDie, _numberOfHitDice) + Abilities.GetAbilityModifier(Ability.Constitution) * _numberOfHitDice;

        SetMaxHPAndCurrentHP(newHitPoints);
    }
    public int GetSkillModifier(Skill skill)
    {
        int skillModifier = skill switch
        {
            Skill.Athletics => Abilities.GetAbilityModifier(Ability.Strength) + SkillProficiencies.GetSkillProficiencyBonus(_proficiencyBonus, Skill.Athletics),
            Skill.Acrobatics => Abilities.GetAbilityModifier(Ability.Dexterity) + SkillProficiencies.GetSkillProficiencyBonus(_proficiencyBonus, Skill.Acrobatics),
            Skill.Perception => Abilities.GetAbilityModifier(Ability.Wisdom) + SkillProficiencies.GetSkillProficiencyBonus(_proficiencyBonus, Skill.Perception),
            Skill.Stealth => Abilities.GetAbilityModifier(Ability.Dexterity) + SkillProficiencies.GetSkillProficiencyBonus(_proficiencyBonus, Skill.Stealth),
            Skill.Performance => Abilities.GetAbilityModifier(Ability.Charisma) + SkillProficiencies.GetSkillProficiencyBonus(_proficiencyBonus, Skill.Performance),
            _ => 0
        };

        return skillModifier;
    }
    public int GetSavingThrowBonus(Ability ability)
    {
        return Abilities.GetAbilityModifier(ability) + (SaveProficiencies.IsProficientInSavesOf(ability) ? _proficiencyBonus : 0);
    }
}
