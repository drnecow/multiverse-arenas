using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MeleeAttackIntDictionary))]
[CustomPropertyDrawer(typeof(RangedAttackIntDictionary))]
[CustomPropertyDrawer(typeof(MonsterActionTypeSpriteDictionary))]
[CustomPropertyDrawer(typeof(AttackTypeSpriteDictionary))]
[CustomPropertyDrawer(typeof(DamageTypeSpriteDictionary))]
[CustomPropertyDrawer(typeof(ConditionSpriteDictionary))]
[CustomPropertyDrawer(typeof(SpecialAbilitySpriteDictionary))]
[CustomPropertyDrawer(typeof(SenseSpriteDictionary))]
[CustomPropertyDrawer(typeof(SpeedSpriteDictionary))]
[CustomPropertyDrawer(typeof(AttackTypeStringDictionary))]
[CustomPropertyDrawer(typeof(ConditionStringDictionary))]
[CustomPropertyDrawer(typeof(MonsterActionTypeStringDictionary))]
[CustomPropertyDrawer(typeof(SpecialAbilityStringDictionary))]
[CustomPropertyDrawer(typeof(SenseStringDictionary))]
[CustomPropertyDrawer(typeof(SpeedStringDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { };