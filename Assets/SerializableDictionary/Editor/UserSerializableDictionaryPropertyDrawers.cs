using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MeleeAttackIntDictionary))]
[CustomPropertyDrawer(typeof(RangedAttackIntDictionary))]
[CustomPropertyDrawer(typeof(MonsterActionTypeSpriteDictionary))]
[CustomPropertyDrawer(typeof(DamageTypeSpriteDictionary))]
[CustomPropertyDrawer(typeof(ConditionSpriteDictionary))]
[CustomPropertyDrawer(typeof(SpecialAbilitySpriteDictionary))]
[CustomPropertyDrawer(typeof(SenseSpriteDictionary))]
[CustomPropertyDrawer(typeof(SpeedSpriteDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { };