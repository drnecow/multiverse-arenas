using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameObject _testMonsterPrefab;
    [SerializeField] private GameObject _testMapPrefab;

    [SerializeField] private InitiativeTracker _initiativeTracker;
    [SerializeField] private PlayerInputSystem _playerInputSystem;

    public HashSet<Monster> AlliedMonsters { get; private set; }
    public HashSet<Monster> EnemyMonsters { get; private set; }
    private Queue<Monster> _initiativeOrder;
    private bool _combatStopped = false;

    public event Action OnNewRoundStarted;
    public event Action<Monster> OnMonsterRemovedFromGame;

    public event Action OnWinGame;
    public event Action OnLoseGame;
    

    private void Awake()
    {
        AlliedMonsters = new HashSet<Monster>();
        EnemyMonsters = new HashSet<Monster>();

        _initiativeOrder = new Queue<Monster>();
    }

    private void Start()
    {
        InitializeCombat();
        ResolveNewTurn();
    }

    private void InitializeCombat()
    {
        // Load map of player choice and set it for CombatDependencies
        GridMap map = Instantiate(_testMapPrefab).GetComponent<GridMap>();
        CombatDependencies.Instance.SetMapAndHighlight(map);


        // Load monsters of player choice, enemies and allies
        for (int i = 0; i < 8; i++)
        {
            GameObject allyPrefab = Instantiate(_testMonsterPrefab);

            Coords allyCoords = new Coords(map.Width / 2, i);
            allyPrefab.transform.position = map.XYToWorldPosition(allyCoords);

            Monster ally = allyPrefab.GetComponent<Monster>();
            AlliedMonsters.Add(ally);
            ally.SetCombatDependencies(CombatDependencies.Instance);
            ally.SetStats(ScriptableObject.Instantiate(ally.Stats));
            map.PlaceMonsterOnCoords(ally, allyCoords);

            ally.IsPlayerControlled = true;
            ally.OnMonsterHPReducedToZero += RemoveMonsterFromGame;
            Destroy(allyPrefab.gameObject.GetComponent<MonsterDecisionController>());
        }
        _playerInputSystem.OnPlayerEndTurn += () => _playerInputSystem.gameObject.SetActive(false);
        _playerInputSystem.OnPlayerEndTurn += ResolveNewTurn;


        for (int j = 0; j < 8; j++)
        {
            GameObject enemyPrefab = Instantiate(_testMonsterPrefab);

            Coords enemyCoords = new Coords(map.Width / 2 + 1, j);
            enemyPrefab.transform.position = map.XYToWorldPosition(enemyCoords);

            Monster enemy = enemyPrefab.GetComponent<Monster>();
            EnemyMonsters.Add(enemy);
            enemy.SetCombatDependencies(CombatDependencies.Instance);
            map.PlaceMonsterOnCoords(enemy, enemyCoords);

            enemy.IsPlayerControlled = false;
            enemy.OnMonsterHPReducedToZero += RemoveMonsterFromGame;
            enemyPrefab.GetComponent<MonsterDecisionController>().OnEnemyEndTurn += ResolveNewTurn;
        }


        // Set all enemies to all allies' visible targets and vice versa
        foreach (Monster enemy in EnemyMonsters)
            foreach (Monster ally in AlliedMonsters)
                ally.VisibleTargets.Add(enemy);

        foreach (Monster ally in AlliedMonsters)
            foreach (Monster enemy in EnemyMonsters)
                enemy.VisibleTargets.Add(ally);


        // Roll initiative for all monsters and place them in queue in initiative order
        Dictionary<Monster, int> monsterInitiative = new Dictionary<Monster, int>();

        foreach (Monster ally in AlliedMonsters)
        {
            int initiativeRoll = ally.RollInitiative();
            monsterInitiative.Add(ally, initiativeRoll);
        }
        foreach (Monster enemy in EnemyMonsters)
        {
            int initiativeRoll = enemy.RollInitiative();
            monsterInitiative.Add(enemy, initiativeRoll);
        }

        monsterInitiative = monsterInitiative.OrderByDescending(entry => entry.Value).ToDictionary(entry => entry.Key, entry => entry.Value);
        _initiativeTracker.SetInitiativeInfo(monsterInitiative, this);
            
        foreach (Monster combatant in monsterInitiative.Keys)
            _initiativeOrder.Enqueue(combatant);
    }
    private void ResolveNewTurn()
    {
        if (!_combatStopped)
        {
            OnNewRoundStarted?.Invoke();

            Monster actor = _initiativeOrder.Dequeue();
            RestoreMonsterResources(actor);

            HashSet<Monster> opposingMonsters = actor.IsPlayerControlled ? EnemyMonsters : AlliedMonsters;
            HashSet<Monster> allyMonsters = actor.IsPlayerControlled ? AlliedMonsters : EnemyMonsters;

            foreach (Monster opposingMonster in opposingMonsters)
            {
                if (!actor.VisibleTargets.Contains(opposingMonster))
                {
                    opposingMonster.IsHiding = true;
                    opposingMonster.MonsterAnimator.SetMonsterStealthMaterial();
                }
                else
                {
                    opposingMonster.IsHiding = false;
                    opposingMonster.MonsterAnimator.SetMonsterNormalMaterial();
                }
            }

            foreach (Monster allyMonster in allyMonsters)
                allyMonster.MonsterAnimator.SetMonsterNormalMaterial();

            if (actor.IsPlayerControlled)
            {
                _playerInputSystem.gameObject.SetActive(true);
                _playerInputSystem.FillActionPanels(actor);
            }
            else
                actor.GetComponent<MonsterDecisionController>().MakeDecision();

            _initiativeOrder.Enqueue(actor);

        }
    }
    private void RestoreMonsterResources(Monster monster)
    {
        monster.MainActionAvailable = true;
        monster.BonusActionAvailable = true;

        monster.RemainingTotalAttacks = monster.NumberOfAttacks;

        foreach (KeyValuePair<MeleeAttack, int> meleeAttackIntPair in monster.CombatActions.MeleeAttacks)
            monster.RemainingAttacks[meleeAttackIntPair.Key] = meleeAttackIntPair.Value;
        foreach (KeyValuePair<RangedAttack, int> rangedAttackIntPair in monster.CombatActions.RangedAttacks)
            monster.RemainingAttacks[rangedAttackIntPair.Key] = rangedAttackIntPair.Value;

        monster.RemainingSpeed.SetSpeedValues(monster.Stats.Speed); // At the start of the monster's turn, its speed restores to maximum

        monster.IsDodging = false;
    }

    public void HandleMonsterEnteringStealth(Monster monster, int stealthRoll)
    {
        HashSet<Monster> oppositeSideMonsters = monster.IsPlayerControlled ? EnemyMonsters : AlliedMonsters;

        foreach (Monster opposingMonster in oppositeSideMonsters)
        {
            int opposingMonsterPerception = opposingMonster.Stats.GetPassiveSkillValue(Skill.Perception);
            
            if (opposingMonsterPerception < stealthRoll)
                opposingMonster.VisibleTargets.Remove(monster);
        }
    }
    public void HandleMonsterBreakingStealth(Monster monster)
    {
        HashSet<Monster> oppositeSideMonsters = monster.IsPlayerControlled ? EnemyMonsters : AlliedMonsters;

        foreach (Monster opposingMonster in oppositeSideMonsters)
            opposingMonster.VisibleTargets.Add(monster);

        monster.MonsterAnimator.SetMonsterNormalMaterial();
    }
    private void RemoveMonsterFromGame(Monster monster)
    {
        Debug.LogWarning($"Removing {monster} from game");
        OnMonsterRemovedFromGame?.Invoke(monster);

        if (monster.IsPlayerControlled)
        {
            AlliedMonsters.Remove(monster);

            foreach (Monster enemy in EnemyMonsters)
                enemy.VisibleTargets.Remove(monster);
        }
        else
        {
            EnemyMonsters.Remove(monster);

            foreach (Monster ally in AlliedMonsters)
                ally.VisibleTargets.Remove(monster);
        }

        if (AlliedMonsters.Count == 0)
        {
            OnLoseGame?.Invoke();
            _combatStopped = true;
        }
        else if (EnemyMonsters.Count == 0)
        {
            OnWinGame?.Invoke();
            _combatStopped = true;
        }

        _initiativeOrder = new Queue<Monster>(_initiativeOrder.Where(actor => actor != monster));

        monster.CombatDependencies.Map.FreeCurrentCoordsOfMonster(monster);
        monster.MonsterAnimator.KillMonster();
        
        Destroy(monster.transform.Find("HPBarUI").gameObject);
        if (!monster.IsPlayerControlled)
            Destroy(monster.gameObject.GetComponent<MonsterDecisionController>());
        monster.enabled = false;
        //Destroy(monster);
    }
}
