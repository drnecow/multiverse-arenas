using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Constants
{
    public enum InfoType
    {
        Neutral = 1,
        Miss = 2,
        CriticalMiss = 3,
        Hit = 4,
        CriticalHit = 5
    }

    public enum Ability
    {
        Strength = 1,
        Dexterity = 2,
        Constitution = 3,
        Intelligence = 4,
        Wisdom = 5,
        Charisma = 6
    }

    public enum Skill
    {
        Athletics = 1,
        Acrobatics = 2,
        Perception = 3,
        Stealth = 4,
        Performance = 5
    }

    public enum ProficiencyType
    {
        None = 1,
        Half = 2,
        Normal = 3,
        Expertise = 4
    }

    public enum Speed
    {
        Walk = 1,
        Fly = 2,
        Swim = 3,
        Climb = 4,
        Burrow = 5
    }

    public enum Sense
    {
        NormalVision = 1,
        Blindsight = 2,
        Darkvision = 3,
        Tremorsense = 4,
        Truesight = 5
    }

    public enum Condition
    {
        Blinded = 1,
        Charmed = 2,
        Deafened = 3,
        Frightened = 4,
        Grappled = 5,
        Incapacitated = 6,
        Invisible = 7,
        Paralyzed = 8,
        Petrified = 9,
        Poisoned = 10,
        Prone = 11,
        Restrained = 12,
        Stunned = 13,
        Unconscious = 14,
        Dodging = 15
    }

    public enum DamageType
    {
        Acid = 1,
        Bludgeoning = 2,
        Cold = 3,
        Fire = 4,
        Force = 5,
        Lightning = 6,
        Necrotic = 7,
        Piercing = 8,
        Poison = 9,
        Psychic = 10,
        Radiant = 11,
        Slashing = 12,
        Thunder = 13
    }

    public enum Size
    {
        Tiny = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
        Huge = 5,
        Gargantuan = 6
    }

    public enum CreatureType
    {
        Aberration = 1,
        Beast = 2,
        Celestial = 3,
        Construct = 4,
        Dragon = 5,
        Elemental = 6,
        Fey = 7,
        Fiend = 8,
        Giant = 9,
        Humanoid = 10,
        Monstrosity = 11,
        Ooze = 12,
        Plant = 13,
        Undead = 14
    }

    public enum SpecialAbility
    {
        Amphibious = 1,
        KeenVision = 2,
        KeenSmell = 3,
        PackTactics = 4
    }

    public enum CombatActionType
    {
        Move = 1,
        Dash = 2,
        Disengage = 3,
        Dodge = 4,
        Grapple = 5,
        Hide = 6,
        Seek = 7
    }

    public enum AttackType
    {
        UnarmedAttack = 1,
        BiteAttack = 2
    }

    public static class ConstantValues
    {
        public const float MAP_CELL_SIZE = 10f;

        public const int MAX_ABILITY_VALUE = 40;
        public const int MIN_PROFICIENCY_BONUS = 2;
        public const int MAX_PROFICIENCY_BONUS = 6;
    }
}
