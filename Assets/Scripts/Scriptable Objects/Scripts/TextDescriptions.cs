using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "TextDescriptions", menuName = "Scriptable Objects/Text/TextDescriptions")]
public class TextDescriptions : ScriptableObject
{
    [SerializeField] private AttackTypeStringDictionary _attackTexts;
    [SerializeField] private ConditionStringDictionary _conditionTexts;
    [SerializeField] private MonsterActionTypeStringDictionary _monsterActionTexts;
    [SerializeField] private SpecialAbilityStringDictionary _specialAbilityTexts;
    [SerializeField] private SenseStringDictionary _senseTexts;
    [SerializeField] private SpeedStringDictionary _speedTexts;

    
    public string GetAttackDescription(AttackType attack)
    {
        if (_attackTexts.ContainsKey(attack))
            return _attackTexts[attack];
        else
            return "Not yet described";
    }
    public string GetConditionDescription(Condition condition)
    {
        if (_conditionTexts.ContainsKey(condition))
            return _conditionTexts[condition];
        else
            return "Not yet described";
    }
    public string GetMonsterActionDescription(MonsterActionType monsterAction)
    {
        if (_monsterActionTexts.ContainsKey(monsterAction))
            return _monsterActionTexts[monsterAction];
        else
            return "Not yet described";
    }
    public string GetSpecialAbilityDescription(SpecialAbility specialAbility)
    {
        if (_specialAbilityTexts.ContainsKey(specialAbility))
            return _specialAbilityTexts[specialAbility];
        else
            return "Not yet described";
    }
    public string GetSenseDescription(Sense sense)
    {
        if (_senseTexts.ContainsKey(sense))
            return _senseTexts[sense];
        else
            return "Not yet described";
    }
    public string GetSpeedDescription(Speed speed)
    {
        if (_speedTexts.ContainsKey(speed))
            return _speedTexts[speed];
        else
            return "Not yet described";
    }
}
