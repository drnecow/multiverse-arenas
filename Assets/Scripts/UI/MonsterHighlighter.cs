using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utils;

public class MonsterHighlighter : MonoBehaviour
{
    Dictionary<Monster, HashSet<MonsterInitiativeInfo>> _monstersWithConnectedInitiativeBlocks;
    Coords _lastMouseCoords = Coords.Zero;


    private void Awake()
    {
        _monstersWithConnectedInitiativeBlocks = new Dictionary<Monster, HashSet<MonsterInitiativeInfo>>();
    }
    private void Update()
    {
        Vector3 mousePosition = Utils.GetMouseWorldPosition();
        Coords mouseCoords = CombatDependencies.Instance.Map.WorldPositionToXY(mousePosition);

        if (mouseCoords != _lastMouseCoords)
        {
            _lastMouseCoords = mouseCoords;

            GridNode targetNode = CombatDependencies.Instance.Map.GetGridObjectAtCoords(mouseCoords);
            Monster targetMonster = null;

            if (targetNode != null)
                targetMonster = targetNode.Monster;

            ClearBlocksHighlight();

            if (targetMonster != null)
                HighlightMonsterBlocks(targetMonster);
        }
    }

    private void HighlightMonsterBlocks(Monster monster)
    {
        if (_monstersWithConnectedInitiativeBlocks.ContainsKey(monster))
            foreach (MonsterInitiativeInfo initiativeBlock in _monstersWithConnectedInitiativeBlocks[monster])
                if (initiativeBlock != null)
                    initiativeBlock.HighlightBlock();
    }
    private void ClearBlocksHighlight()
    {
        foreach (HashSet<MonsterInitiativeInfo> monsterBlocks in _monstersWithConnectedInitiativeBlocks.Values)
        {
            foreach (MonsterInitiativeInfo initiativeBlock in monsterBlocks)
                if (initiativeBlock != null)
                    initiativeBlock.ClearBlockHighlight();
        }
    }
    public void AddMonsterInitiativeBlock(Monster monster, MonsterInitiativeInfo initiativeBlock)
    {
        if (_monstersWithConnectedInitiativeBlocks.ContainsKey(monster))
            _monstersWithConnectedInitiativeBlocks[monster].Add(initiativeBlock);
        else
        {
            HashSet<MonsterInitiativeInfo> monsterInitiativeBlocks = new HashSet<MonsterInitiativeInfo>() { initiativeBlock };
            _monstersWithConnectedInitiativeBlocks.Add(monster, monsterInitiativeBlocks);
        }
    }
    public void RemoveMonster(Monster monster)
    {
        _monstersWithConnectedInitiativeBlocks.Remove(monster);
    }
}
