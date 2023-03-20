using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeleeAttackIntDictionary : SerializableDictionary<MeleeAttack, int> { };

[System.Serializable]
public class RangedAttackIntDictionary : SerializableDictionary<RangedAttack, int> { };