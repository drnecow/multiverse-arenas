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

    private Queue<Monster> _initiativeOrder;
    private LinkedList<GameObject> _displayedGameObjects;

    private bool _isFirstRound;
    private int _nextRound;
    private Monster _firstMonsterInInitiative;


    private void Awake()
    {
        _contentParent = GetComponent<RectTransform>();    
    }

    public void SetInitiativeInfo(Queue<Monster> initiativeOrder, CombatManager combatManager)
    {
        // Create queue of monsters in initiative order
        // Mark first monster in initiative
        // Dequeue monsters up to queue length

        List<Monster> allMonsters = initiativeOrder.ToList();
        Debug.Log("All initiatives:");
        foreach (Monster monster in allMonsters)
            Debug.Log($"{monster}: {monster.InitiativeRoll}");

        _isFirstRound = true;
        _nextRound = 2;
        _initiativeOrder = new Queue<Monster>(initiativeOrder);
        _displayedGameObjects = new LinkedList<GameObject>();

        _firstMonsterInInitiative = _initiativeOrder.Peek();

        int currentCell = 1;

        while (currentCell < INITIATIVE_TRACKER_SIZE)
        {
            int numberOfMonsters = _initiativeOrder.Count;

            for (int i = 0; i < Mathf.Min(INITIATIVE_TRACKER_SIZE, numberOfMonsters); i++)
            {
                Monster currentMonster = _initiativeOrder.Dequeue();

                InsertInitiativeInfoBlock(currentMonster);

                _initiativeOrder.Enqueue(currentMonster);
                currentCell++;
            }
            
            //Debug.Log($"Current cell: {currentCell}");

            if (currentCell < INITIATIVE_TRACKER_SIZE)
            {
                InsertRoundSeparator();
                currentCell++;
            }
        }

        combatManager.OnNewRoundStarted += MoveToNextRound;
        combatManager.OnMonsterRemovedFromGame += DeleteMonster;

        combatManager.OnWinGame += ClearAllElements;
        combatManager.OnLoseGame += ClearAllElements;
    }
    public void MoveToNextRound()
    {
        if (_isFirstRound)
        {
            _isFirstRound = false;
            return;
        }

        do
        {
            DeleteFirstElement();
            InsertNextBlock();
        }
        while (IsFirstElementRoundSeparator());
    }
    private void DeleteMonster(Monster deletedMonster)
    {
        Debug.LogWarning("Deleting monster from initiative tracker");

        // Remove deleted monster from initiative queue
        // Remove all info blocks of deleted monster from displayed objects and record how many there were
        // If deleted monster happens to be first in initiative, convert initiative queue to list and find monster with next-highest initiative in it
        // Add elements equal to the number of deleted monster blocks

        _initiativeOrder = new Queue<Monster>(_initiativeOrder.Where(monster => monster != deletedMonster));

        if (_initiativeOrder.Count == 0)
        {
            ClearAllElements();
            return;
        }

        List<GameObject> blocksToDelete = new List<GameObject>();

        foreach (GameObject block in _displayedGameObjects)
        {
            MonsterInitiativeInfo blockInfo = block.GetComponent<MonsterInitiativeInfo>();

            if (blockInfo != null)
                if (blockInfo.Monster == deletedMonster)
                    blocksToDelete.Add(block);
        }

        int numberOfEmptyBlocks = blocksToDelete.Count;

        foreach (GameObject blockToDelete in blocksToDelete)
        {
            _displayedGameObjects.Remove(blockToDelete);
            Destroy(blockToDelete);
        }

        if (deletedMonster == _firstMonsterInInitiative)
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

        for (int i = 0; i < numberOfEmptyBlocks; i++)
            InsertNextBlock();
    }

    private void DeleteFirstElement()
    {
        GameObject firstElement = _displayedGameObjects.First();
        _displayedGameObjects.RemoveFirst();
        Destroy(firstElement);
    }
    private void ClearAllElements()
    {
        foreach (GameObject block in _displayedGameObjects)
        {
            Destroy(block);
        }
        _displayedGameObjects.Clear();
    }
    private void InsertInitiativeInfoBlock(Monster monster)
    {
        GameObject infoItem;

        if (monster.IsPlayerControlled)
            infoItem = Instantiate(_friendlyInitiativeInfoPrefab);
        else
            infoItem = Instantiate(_enemyInitiativeInfoPrefab);

        infoItem.transform.SetParent(_contentParent, false);
        _displayedGameObjects.AddLast(infoItem);

        MonsterInitiativeInfo info = infoItem.GetComponent<MonsterInitiativeInfo>();
        info.SetInfo(monster);

        CombatDependencies.Instance.MonsterHighlighter.AddMonsterInitiativeBlock(monster, info);
    }
    private void InsertRoundSeparator()
    {
        GameObject roundSeparator = Instantiate(_roundSeparatorPrefab);
        roundSeparator.transform.SetParent(_contentParent, false);
        _displayedGameObjects.AddLast(roundSeparator);

        roundSeparator.GetComponentInChildren<TextMeshProUGUI>().text = $"Round {_nextRound}";
        _nextRound++;
    }
    private void InsertNextBlock()
    {
        // Peek the next monster
        Monster nextMonster = _initiativeOrder.Peek();
        // If next monster is first monster in initiative and last element is not round separator, insert round separator
        if (nextMonster == _firstMonsterInInitiative && !IsLastElementRoundSeparator())
        {
            InsertRoundSeparator();
        }
        // Else, dequeue next monster and insert it as last element; enqueue dequeued monster again
        else
        {
            nextMonster = _initiativeOrder.Dequeue();
            InsertInitiativeInfoBlock(nextMonster);
            _initiativeOrder.Enqueue(nextMonster);
        }
    }
    private bool IsFirstElementRoundSeparator()
    {
        GameObject firstElement = _displayedGameObjects.First();

        if (firstElement.GetComponent<MonsterInitiativeInfo>() == null)
            return true;
        else
            return false;
    }
    private bool IsLastElementRoundSeparator()
    {
        GameObject lastElement = _displayedGameObjects.Last();
        
        if (lastElement.GetComponent<MonsterInitiativeInfo>() == null)
            return true;
        else
            return false;
    }
}
