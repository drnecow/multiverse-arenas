using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[CreateAssetMenu(fileName = "VisualAssets", menuName = "Scriptable Objects/Visual/VisualAssets")]
public class VisualAssets : ScriptableObject
{
    [field: SerializeField] public Material NormalMaterial { get; private set; }
    [field: SerializeField] public Material StealthMaterial { get; private set; }

    [SerializeField] private Sprite _defaultSprite;

    [SerializeField] private MonsterActionTypeSpriteDictionary _monsterActionSprites;
    [SerializeField] private DamageTypeSpriteDictionary _damageTypeSprites;
    [SerializeField] private ConditionSpriteDictionary _conditionSprites;
    [SerializeField] private SpecialAbilitySpriteDictionary _specialAbilitySprites;
    [SerializeField] private SenseSpriteDictionary _senseSprites;
    [SerializeField] private SpeedSpriteDictionary _speedSprites;

    
    public Sprite GetSpriteForMonsterAction(MonsterActionType monsterAction)
    {
        if (_monsterActionSprites.ContainsKey(monsterAction))
            return _monsterActionSprites[monsterAction];
        else
            return _defaultSprite;
    }
    public Sprite GetSpriteForDamageType(DamageType damageType)
    {
        if (_damageTypeSprites.ContainsKey(damageType))
            return _damageTypeSprites[damageType];
        else
            return _defaultSprite;
    }
    public Sprite GetSpriteForCondition(Condition condition)
    {
        if (_conditionSprites.ContainsKey(condition))
            return _conditionSprites[condition];
        else
            return _defaultSprite;
    }
    public Sprite GetSpriteForSpecialAbility(SpecialAbility specialAbility)
    {
        if (_specialAbilitySprites.ContainsKey(specialAbility))
            return _specialAbilitySprites[specialAbility];
        else
            return _defaultSprite;
    }
    public Sprite GetSpriteForSense(Sense sense)
    {
        if (_senseSprites.ContainsKey(sense))
            return _senseSprites[sense];
        else
            return _defaultSprite;
    }
    public Sprite GetSpriteForSpeed(Speed speed)
    {
        if (_speedSprites.ContainsKey(speed))
            return _speedSprites[speed];
        else
            return _defaultSprite;
    }
}
