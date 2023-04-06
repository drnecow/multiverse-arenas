using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class InitiativeTracker : MonoBehaviour
{
    const int INITIATIVE_TRACKER_SIZE = 15;

    private RectTransform _contentParent;

    [SerializeField] private GameObject _friendlyInitiativeInfoPrefab;
    [SerializeField] private GameObject _enemyInitiativeInfoPrefab;
    [SerializeField] private GameObject _roundSeparatorPrefab;

    private Dictionary<Monster, int> _initiativesList;
    private Queue<Monster> _initiativeOrder;
    private Queue<GameObject> _displayedGameObjects;
    private List<MonsterInitiativeInfo> _displayedInitiativeInfo;

    private int _nextRound;
    private Monster _firstMonsterInInitiative;


    private void Awake()
    {
        _contentParent = GetComponent<RectTransform>();    
    }

    public void SetInitiativeInfo(Dictionary<Monster, int> initiativesList, CombatManager combatManager)
    {
        // Create queue of monsters in initiative order
        // Mark first monster in initiative
        // Dequeue monsters up to queue length

        _nextRound = 2;
        _initiativesList = initiativesList;
        _initiativeOrder = new Queue<Monster>();
        _displayedGameObjects = new Queue<GameObject>();
        _displayedInitiativeInfo = new List<MonsterInitiativeInfo>();

        foreach (Monster monster in initiativesList.Keys)
            _initiativeOrder.Enqueue(monster);

        _firstMonsterInInitiative = _initiativeOrder.Peek();
        List<Monster> allMonsters = initiativesList.Keys.ToList();

        int currentCell = 0;

        while (currentCell < INITIATIVE_TRACKER_SIZE)
        {
            for (int i = 0; i < Mathf.Min(INITIATIVE_TRACKER_SIZE, allMonsters.Count); i++)
            {
                Monster currentMonster = _initiativeOrder.Dequeue();
                GameObject infoItem;

                if (currentMonster.IsPlayerControlled)
                    infoItem = Instantiate(_friendlyInitiativeInfoPrefab);
                else
                    infoItem = Instantiate(_enemyInitiativeInfoPrefab);

                infoItem.transform.SetParent(_contentParent);
                _displayedGameObjects.Enqueue(infoItem);

                MonsterInitiativeInfo info = infoItem.GetComponent<MonsterInitiativeInfo>();
                info.SetInfo(currentMonster, initiativesList[allMonsters[i]]);

                _initiativeOrder.Enqueue(currentMonster);
                _displayedInitiativeInfo.Add(info);
                currentCell++;
            }
            
            Debug.Log($"Current cell: {currentCell}");

            if (currentCell < INITIATIVE_TRACKER_SIZE)
            {
                GameObject roundSeparator = Instantiate(_roundSeparatorPrefab);
                roundSeparator.transform.SetParent(_contentParent);
                _displayedGameObjects.Enqueue(roundSeparator);

                roundSeparator.GetComponentInChildren<TextMeshProUGUI>().text = $"Round {_nextRound}";
                _nextRound++;
                currentCell++;
            }
        }

        combatManager.OnNewRoundStarted += MoveToNextRound;
        combatManager.OnMonsterRemovedFromGame += DeleteMonster;
    }
    // FIXME: something is wrong here
    private void MoveToNextRound()
    {
        // When the next round comes, delete first element
        // If firsts element is round separator, delete first element again
        // Dequeue
        // If dequeued element is first monster, insert separator as last element and increment next round
        // Insert dequeued monster as last element
        // Enqueue dequeued monster again

        GameObject firstElement = _displayedGameObjects.Dequeue();
        MonsterInitiativeInfo elementInitiativeInfo = firstElement.GetComponent<MonsterInitiativeInfo>();

        if (elementInitiativeInfo == null) // first element in initiative tracker is round separator
        {
            GameObject secondElement = _displayedGameObjects.Dequeue();
            elementInitiativeInfo = secondElement.GetComponent<MonsterInitiativeInfo>();

            _displayedInitiativeInfo.RemoveAt(0);
            _initiativeOrder.Enqueue(elementInitiativeInfo.Monster);

            Destroy(secondElement);
        }
        else
        {
            _displayedInitiativeInfo.RemoveAt(0);
            _initiativeOrder.Enqueue(elementInitiativeInfo.Monster);
        }
        Destroy(firstElement);


        Monster nextMonster = _initiativeOrder.Dequeue();

        if (nextMonster == _firstMonsterInInitiative)
        {
            GameObject roundSeparator = Instantiate(_roundSeparatorPrefab);
            roundSeparator.transform.SetParent(_contentParent);
            _displayedGameObjects.Enqueue(roundSeparator);

            roundSeparator.GetComponentInChildren<TextMeshProUGUI>().text = $"Round {_nextRound}";
            _nextRound++;
        }

        GameObject infoItem;

        if (nextMonster.IsPlayerControlled)
            infoItem = Instantiate(_friendlyInitiativeInfoPrefab);
        else
            infoItem = Instantiate(_enemyInitiativeInfoPrefab);

        infoItem.transform.SetParent(_contentParent);
        _displayedGameObjects.Enqueue(infoItem);

        MonsterInitiativeInfo info = infoItem.GetComponent<MonsterInitiativeInfo>();
        info.SetInfo(nextMonster, _initiativesList[nextMonster]);

        _initiativeOrder.Enqueue(nextMonster);
        _displayedInitiativeInfo.Add(info);
    }
    // FIXME: ...and here
    private void DeleteMonster(Monster deletedMonster)
    {
        Debug.LogWarning("Deleting monster from initiative tracker");
        if (deletedMonster == _firstMonsterInInitiative)
        {
            int highestInitiative = -1000;
            foreach (Monster monster in _initiativesList.Keys)
            {
                if (monster != deletedMonster && _initiativesList[monster] > highestInitiative)
                {
                    _firstMonsterInInitiative = monster;
                    highestInitiative = _initiativesList[monster];
                }
            }

            Debug.Log($"Monster with next-highest initiative: {_firstMonsterInInitiative} ({_initiativesList[deletedMonster]}");
        }

        _initiativesList = _initiativesList.Where(keyValuePair => keyValuePair.Key != deletedMonster).ToDictionary(enty => enty.Key, enty => enty.Value);
        _initiativeOrder = new Queue<Monster>(_initiativeOrder.Where(element => element != deletedMonster));

        foreach (GameObject gameObj in _displayedGameObjects)
        {
            MonsterInitiativeInfo objInitiativeInfo = gameObj.GetComponent<MonsterInitiativeInfo>();
            
            if (objInitiativeInfo != null)
                if (objInitiativeInfo.Monster == deletedMonster)
                    Destroy(gameObj);
        }

        _displayedInitiativeInfo = _displayedInitiativeInfo.Where(info => info.Monster != deletedMonster).ToList();
        _displayedGameObjects = new Queue<GameObject>(_displayedGameObjects.Where(gameObj => gameObj.GetComponent<MonsterInitiativeInfo>()?.Monster != deletedMonster));
    }
}
