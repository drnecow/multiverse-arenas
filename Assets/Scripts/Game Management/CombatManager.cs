using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameObject _giantRatPrefab;
    [SerializeField] private GameObject _mapPrefab;

    [SerializeField] private CameraController _camera;
    [SerializeField] private InitiativeTracker _initiativeTracker;
    [SerializeField] private CharacterSheet _characterSheet;
    [SerializeField] private PlayerInputSystem _playerInputSystem;
    [SerializeField] private SceneManager _sceneManager;

    public HashSet<Monster> AlliedMonsters { get; private set; }
    public HashSet<Monster> EnemyMonsters { get; private set; }
    private Queue<Monster> _initiativeOrder;
    int _currentRound = 0;
    private Monster _firstMonsterInInitiative;
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

        OnWinGame += () => _playerInputSystem.gameObject.SetActive(false);
        OnLoseGame += () => _playerInputSystem.gameObject.SetActive(false);

        OnWinGame += () => StartCoroutine(ExitToMainMenuWithDelay());
        OnLoseGame += () => StartCoroutine(ExitToMainMenuWithDelay());
    }

    private void Start()
    {
        InitializeCombat();
        ResolveNewTurn();
    }

    private void InitializeCombat()
    {
        // Load map of player choice and set it for CombatDependencies
        GridMap map = Instantiate(_mapPrefab).GetComponent<GridMap>();
        CombatDependencies.Instance.SetMapAndHighlight(map);

        int allyRats = PlayerData.Instance == null ? 1 : PlayerData.Instance.AllyRats;
        // Load monsters of player choice, enemies and allies
        for (int i = map.AllyStartingPoint.y; i < map.AllyStartingPoint.y + allyRats; i++)
        {
            GameObject allyPrefab = Instantiate(_giantRatPrefab);

            Monster ally = allyPrefab.GetComponent<Monster>();
            AlliedMonsters.Add(ally);
            ally.SetCombatDependencies(CombatDependencies.Instance);
            ally.SetStats(ScriptableObject.Instantiate(ally.Stats));

            Coords allyCoords = new Coords(map.AllyStartingPoint.x, i);
            allyPrefab.transform.position = GridMap.GetCenterOfSquare(allyCoords, GridMap.GetSquareSideForEntitySize(ally.Stats.Size));
            allyPrefab.GetComponent<SpriteRenderer>().flipX = true;
            map.PlaceMonsterOnCoords(ally, allyCoords);

            ally.IsPlayerControlled = true;
            ally.OnMonsterHPReducedToZero += RemoveMonsterFromGame;
            Destroy(allyPrefab.gameObject.GetComponent<MonsterDecisionController>());
        }
        _playerInputSystem.OnPlayerEndTurn += () => _playerInputSystem.gameObject.SetActive(false);
        _playerInputSystem.OnPlayerEndTurn += ResolveNewTurn;


        int enemyRats = PlayerData.Instance == null ? 1 : PlayerData.Instance.EnemyRats;
        for (int j = map.EnemyStartingPoint.y; j < map.EnemyStartingPoint.y + enemyRats; j++)
        {
            GameObject enemyPrefab = Instantiate(_giantRatPrefab);
            enemyPrefab.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);

            Monster enemy = enemyPrefab.GetComponent<Monster>();
            EnemyMonsters.Add(enemy);
            enemy.SetCombatDependencies(CombatDependencies.Instance);

            Coords enemyCoords = new Coords(map.EnemyStartingPoint.x, j);
            enemyPrefab.transform.position = GridMap.GetCenterOfSquare(enemyCoords, GridMap.GetSquareSideForEntitySize(enemy.Stats.Size));
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
            ally.InitiativeRoll = initiativeRoll;
            monsterInitiative.Add(ally, initiativeRoll);
        }
        foreach (Monster enemy in EnemyMonsters)
        {
            int initiativeRoll = enemy.RollInitiative();
            enemy.InitiativeRoll = initiativeRoll;
            monsterInitiative.Add(enemy, initiativeRoll);
        }

        monsterInitiative = monsterInitiative.OrderByDescending(entry => entry.Value).ToDictionary(entry => entry.Key, entry => entry.Value);
            
        foreach (Monster combatant in monsterInitiative.Keys)
            _initiativeOrder.Enqueue(combatant);

        _firstMonsterInInitiative = _initiativeOrder.Peek();
        _initiativeTracker.SetInitiativeInfo(_initiativeOrder, this);
    }
    private void ResolveNewTurn()
    {
        if (!_combatStopped)
        {
            EventLogsChat chat = CombatDependencies.Instance.EventsLogger.Chat;

            OnNewRoundStarted?.Invoke();

            Monster actor = _initiativeOrder.Dequeue();

            if (actor == _firstMonsterInInitiative)
            {
                _currentRound++;
                chat.LogRoundSeparator(_currentRound);
            }

            chat.LogMonsterNewTurn(actor);
            _characterSheet.SetPinnedMonsterAndItsInfo(actor);
            RestoreMonsterResources(actor);

            StartCoroutine(AnnounceMonsterTurn(actor));

            _initiativeOrder.Enqueue(actor);
        }
    }
    private IEnumerator AnnounceMonsterTurn(Monster monster)
    {
        _camera.FocusCameraOnMonster(monster);
        LogColor logColor = monster.IsPlayerControlled ? LogColor.Hit : LogColor.Miss;
        CombatDependencies.Instance.EventsLogger.LogScreenInfo($"{monster.Name}'s turn", logColor);

        StartCoroutine(HighlightActiveMonster(monster));

        yield return new WaitForSeconds(0.7f);

        if (monster.IsPlayerControlled)
        {
            _playerInputSystem.gameObject.SetActive(true);
            _playerInputSystem.FillActionPanels(monster);
        }
        else
            monster.GetComponent<MonsterDecisionController>().MakeDecision();
    }
    private IEnumerator HighlightActiveMonster(Monster monster)
    {
        CombatDependencies.Instance.Highlight.HighlightCells(monster.CurrentCoords, Color.yellow);
        yield return new WaitForSeconds(2f);
        CombatDependencies.Instance.Highlight.ClearHighlight();
    }
    private IEnumerator ExitToMainMenuWithDelay()
    {
        yield return new WaitForSeconds(2f);
        _sceneManager.LoadScene(Scene.MainMenu);
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

        monster.RemoveActiveCondition(Condition.Dodging);
    }

    public void HandleMonsterEnteringStealth(Monster monster)
    {
        HashSet<Monster> oppositeSideMonsters = monster.IsPlayerControlled ? EnemyMonsters : AlliedMonsters;

        foreach (Monster opposingMonster in oppositeSideMonsters)
            opposingMonster.VisibleTargets.Remove(monster);
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
        OnMonsterRemovedFromGame?.Invoke(monster);
        CombatDependencies.Instance.MonsterHighlighter.RemoveMonster(monster);

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
        if (monster == _firstMonsterInInitiative)
        {
            List<Monster> remainingMonsters = _initiativeOrder.ToList();

            int highestInitiativeRoll = -1000;
            foreach (Monster remainingMonster in remainingMonsters)
            {
                if (remainingMonster.InitiativeRoll > highestInitiativeRoll)
                {
                    highestInitiativeRoll = remainingMonster.InitiativeRoll;
                    _firstMonsterInInitiative = remainingMonster;
                }
            }
        }

        monster.CombatDependencies.Map.FreeCurrentCoordsOfMonster(monster);
        monster.MonsterAnimator.KillMonster();
        
        Destroy(monster.transform.Find("HPBarUI").gameObject);
        if (!monster.IsPlayerControlled)
            Destroy(monster.gameObject.GetComponent<MonsterDecisionController>());
        monster.enabled = false;
        //Destroy(monster);
    }
}
