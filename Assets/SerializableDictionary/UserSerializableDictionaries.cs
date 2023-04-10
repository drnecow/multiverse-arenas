using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

[Serializable]
public class MeleeAttackIntDictionary : SerializableDictionary<MeleeAttack, int> { };

[Serializable]
public class RangedAttackIntDictionary : SerializableDictionary<RangedAttack, int> { };

[Serializable]
public class MonsterActionTypeSpriteDictionary: SerializableDictionary<MonsterActionType, Sprite> { }

[Serializable]
public class DamageTypeSpriteDictionary : SerializableDictionary<DamageType, Sprite> { };

[Serializable]
public class ConditionSpriteDictionary : SerializableDictionary<Condition, Sprite> { };

[Serializable]
public class SpecialAbilitySpriteDictionary : SerializableDictionary<SpecialAbility, Sprite> { };

[Serializable]
public class SenseSpriteDictionary : SerializableDictionary<Sense, Sprite> { };

[Serializable]
public class SpeedSpriteDictionary : SerializableDictionary<Speed, Sprite> { };