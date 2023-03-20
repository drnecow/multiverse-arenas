using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

namespace Project.DictionaryStructs
{
    [System.Serializable]
    public class Abilities
    {
        [SerializeField] private int _strength;
        [SerializeField] private int _dexterity;
        [SerializeField] private int _constitution;
        [SerializeField] private int _intelligence;
        [SerializeField] private int _wisdom;
        [SerializeField] private int _charisma;

        public int Strength
        {
            get => _strength;
            set
            {
                if (value >= 0 && value <= ConstantValues.MAX_ABILITY_VALUE)
                    _strength = value;
                else
                    Debug.LogWarning($"Invalid strength value: {value}");
            }
        }
        public int Dexterity
        {
            get => _dexterity;
            set
            {
                if (value >= 0 && value <= ConstantValues.MAX_ABILITY_VALUE)
                    _dexterity = value;
                else
                    Debug.LogWarning($"Invalid dexterity value: {value}");
            }
        }
        public int Constitution
        {
            get => _constitution;
            set
            {
                if (value >= 0 && value <= ConstantValues.MAX_ABILITY_VALUE)
                    _constitution = value;
                else
                    Debug.LogWarning($"Invalid constitution value: {value}");
            }
        }
        public int Intelligence
        {
            get => _intelligence;
            set
            {
                if (value >= 0 && value <= ConstantValues.MAX_ABILITY_VALUE)
                    _intelligence = value;
                else
                    Debug.LogWarning($"Invalid intelligence value: {value}");
            }
        }
        public int Wisdom
        {
            get => _wisdom;
            set
            {
                if (value >= 0 && value <= ConstantValues.MAX_ABILITY_VALUE)
                    _wisdom = value;
                else
                    Debug.LogWarning($"Invalid wisdom value: {value}");
            }
        }
        public int Charisma
        {
            get => _charisma;
            set
            {
                if (value >= 0 && value <= ConstantValues.MAX_ABILITY_VALUE)
                    _charisma = value;
                else
                    Debug.LogWarning($"Invalid charisma value: {value}");
            }
        }

        public int GetAbilityModifier(Ability ability)
        {
            int targetAbility = ability switch
            {
                Ability.Strength => _strength,
                Ability.Dexterity => _dexterity,
                Ability.Constitution => _constitution,
                Ability.Intelligence => _intelligence,
                Ability.Wisdom => _wisdom,
                Ability.Charisma => _charisma,
                _ => 0
            };

            return Mathf.FloorToInt((targetAbility - 10) / 2);
        }
    }

    [System.Serializable]
    public class SkillProficiencies
    {
        [SerializeField] private ProficiencyType _athletics = ProficiencyType.None;
        [SerializeField] private ProficiencyType _acrobatics = ProficiencyType.None;
        [SerializeField] private ProficiencyType _perception = ProficiencyType.None;
        [SerializeField] private ProficiencyType _stealth = ProficiencyType.None;
        [SerializeField] private ProficiencyType _performance = ProficiencyType.None;

        public ProficiencyType Athletics { get => _athletics; set => _athletics = value; }
        public ProficiencyType Acrobatics { get => _acrobatics; set => _acrobatics = value; }
        public ProficiencyType Perception { get => _perception; set => _perception = value; }
        public ProficiencyType Stealth { get => _stealth; set => _stealth = value; }
        public ProficiencyType Performance { get => _performance; set => _performance = value; }

        public int GetSkillProficiencyBonus(int monsterProficiencyBonus, Skill skill)
        {
            ProficiencyType targetSkillProficiency = skill switch
            {
                Skill.Athletics => _athletics,
                Skill.Acrobatics => _acrobatics,
                Skill.Perception => _perception,
                Skill.Stealth => _stealth,
                Skill.Performance => _performance,
                _ => 0
            };

            int skillProficiencyBonus = targetSkillProficiency switch
            {
                ProficiencyType.None => 0,
                ProficiencyType.Half => monsterProficiencyBonus / 2,
                ProficiencyType.Normal => monsterProficiencyBonus,
                ProficiencyType.Expertise => monsterProficiencyBonus * 2,
                _ => 0
            };

            return skillProficiencyBonus;
        }
    }

    [System.Serializable]
    public class SavingThrowProficiencies
    {
        [SerializeField] private bool _strength;
        [SerializeField] private bool _dexterity;
        [SerializeField] private bool _constitution;
        [SerializeField] private bool _intelligence;
        [SerializeField] private bool _wisdom;
        [SerializeField] private bool _charisma;


        public bool IsProficientInSavesOf(Ability ability)
        {
            bool isProficient = ability switch
            {
                Ability.Strength => _strength,
                Ability.Dexterity => _dexterity,
                Ability.Constitution => _constitution,
                Ability.Intelligence => _intelligence,
                Ability.Wisdom => _wisdom,
                Ability.Charisma => _charisma,
                _ => false
            };

            return isProficient;
        }
    }

    [System.Serializable]
    public class SpeedValues
    {
        [SerializeField] private int _walk;
        [SerializeField] private int _fly;
        [SerializeField] private int _swim;
        [SerializeField] private int _climb;
        [SerializeField] private int _burrow;

        public int Walk
        {   
            get => _walk; 
            set
            {
                if (value >= 0)
                    _walk = value;
                else
                    _walk = 0;
            } 
        }
        public int Fly
        {
            get => _fly;
            set
            {
                if (value >= 0)
                    _fly = value;
                else
                    _fly = 0;
            }
        }
        public int Swim
        {
            get => _swim;
            set
            {
                if (value >= 0)
                    _swim = value;
                else
                    _swim = 0;
            }
        }
        public int Climb
        {
            get => _climb;
            set
            {
                if (value >= 0)
                    _climb = value;
                else
                    _climb = 0;
            }
        }
        public int Burrow
        {
            get => _burrow;
            set
            {
                if (value >= 0)
                    _burrow = value;
                else
                    _burrow = 0;
            }
        }
    }

    [System.Serializable]
    public class Senses
    {
        [SerializeField] private int _normalVision;
        [SerializeField] private int _blindsight;
        [SerializeField] private int _darkvision;
        [SerializeField] private int _tremorsense;
        [SerializeField] private int _truesight;

        public int NormalVision
        {
            get => _normalVision;
            set
            {
                if (value >= 0)
                    _normalVision = value;
                else
                    _normalVision = 0;
            }
        }
        public int Blindsight
        {
            get => _blindsight;
            set
            {
                if (value >= 0)
                    _blindsight = value;
                else
                    _blindsight = 0;
            }
        }
        public int Darkvision
        {
            get => _darkvision;
            set
            {
                if (value >= 0)
                    _darkvision = value;
                else
                    _darkvision = 0;
            }
        }
        public int Tremorsense
        {
            get => _tremorsense;
            set
            {
                if (value >= 0)
                    _tremorsense = value;
                else
                    _tremorsense = 0;
            }
        }
        public int Truesight
        {
            get => _truesight;
            set
            {
                if (value >= 0)
                    _truesight = value;
                else
                    _truesight = 0;
            }
        }
    }
}
