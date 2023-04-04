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
        for (int i = 0; i < map.Height; i++)
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

            // Subscribe removal of Disengage to player input system's OnPlayerEndTurn
            // Subscribe ResolveNewTurn() to player input system's OnPlayerEndTurn

        }
        _playerInputSystem.OnPlayerEndTurn += () => _playerInputSystem.gameObject.SetActive(false);
        _playerInputSystem.OnPlayerEndTurn += ResolveNewTurn;


        for (int j = 0; j < map.Height; j++)
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
        _initiativeTracker.SetInitiativeInfo(monsterInitiative);
            
        foreach (Monster combatant in monsterInitiative.Keys)
            _initiativeOrder.Enqueue(combatant);
    }
    private void ResolveNewTurn()
    {
        if (!_combatStopped)
        {
            Monster actor = _initiativeOrder.Dequeue();
            RestoreMonsterResources(actor);

            HashSet<Monster> opposingMonsters = actor.IsPlayerControlled ? EnemyMonsters : AlliedMonsters;
            foreach (Monster opposingMonster in opposingMonsters)
            {
                if (!actor.VisibleTargets.Contains(opposingMonster))
                    opposingMonster.IsHiding = true;
                else
                    opposingMonster.IsHiding = false;
            }

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
    }
    private void RemoveMonsterFromGame(Monster monster)
    {
        Debug.LogWarning($"Removing {monster} from game");

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
        monster.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Dead monsters";
        Destroy(monster.transform.Find("HPBarUI").gameObject);
        Destroy(monster.gameObject.GetComponent<MonsterDecisionController>());
        //monster.gameObject.SetActive(false);
        Destroy(monster);
    }
}
