using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackRowInfo : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI AttackName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI AttackDistance { get; private set; }
    [field: SerializeField] public TextMeshProUGUI ToHitBonus { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Damage { get; private set; }
}
