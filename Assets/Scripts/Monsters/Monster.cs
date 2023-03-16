using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;

public class Monster : MonoBehaviour
{
    [field: SerializeField] public MonsterStats Stats { get; private set; }
    private int _numberOfAttacks;

    [SerializeField] private List<CombatAction> _freeActions;
    [SerializeField] private List<CombatAction> _mainActions;
    [SerializeField] private List<CombatAction> _bonusActions;

    public HashSet<Condition> ActiveConditions { get; private set; }
    private bool _mainActionUsed = false;
    public bool MainActionUsed { get => _mainActionUsed; set => _mainActionUsed = value; }
    private bool _bonusActionUsed = false;
    public bool BonusActionUsed { get => _bonusActionUsed; set => _bonusActionUsed = value; }
    private bool _reactionUsed = false;
    public bool ReactionUsed { get => _reactionUsed; set => _reactionUsed = value; }


    public void AddActiveCondition(Condition condition)
    {
        ActiveConditions.Add(condition);
    }

    private void Awake()
    {
        ActiveConditions = new HashSet<Condition>();
    }
}
