using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Utils;
using Project.Constants;

public class TestPathfinding : MonoBehaviour
{
    [SerializeField] private MapHighlight _highlight;
    [SerializeField] private GridMap _map;

    [SerializeField] private Monster _monster;
    private Coords _currentMouseCoords;


    private void Start()
    {
        CombatDependencies.Instance.SetMapAndHighlight(_map);
        _monster.SetCombatDependencies(CombatDependencies.Instance);
        _map.PlaceMonsterOnCoords(_monster, _monster.CurrentCoordsOriginCell);

        _currentMouseCoords = _map.WorldPositionToXY(Utils.GetMouseWorldPosition());
        List<Coords> path = _map.FindPathForMonster(_monster, _currentMouseCoords);

        if (path != null)
        {
            _highlight.ClearHighlight();
            _highlight.HighlightCells(_monster.CurrentCoords, Color.green);

            foreach (Coords coord in path)
                _highlight.HighlightCell(coord, Color.green);
        }
    }

    private void Update()
    {
        Coords mouseCoords = _map.WorldPositionToXY(Utils.GetMouseWorldPosition());

        if ((mouseCoords != _currentMouseCoords) && _map.ValidateCoords(mouseCoords))
        {
            _currentMouseCoords = mouseCoords;
            List<Coords> path = _map.FindPathForMonster(_monster, _currentMouseCoords);

            if (path != null)
            {
                Debug.Log($"Destination cell: {_currentMouseCoords}");
                Debug.Log("Path:");
                foreach (Coords cell in path)
                    Debug.Log(cell);

                Size monsterSize = _monster.Stats.Size;
                HashSet<Coords> allCellsPath = new HashSet<Coords>();

                foreach (Coords pathOriginCell in path)
                {
                    List<Coords> pathCells = _map.GetListOfMonsterCoords(pathOriginCell, monsterSize);

                    foreach (Coords pathCell in pathCells)
                        allCellsPath.Add(pathCell);
                }

                _highlight.ClearHighlight();
                _highlight.HighlightCells(_monster.CurrentCoords, Color.green);
                _highlight.HighlightCells(allCellsPath.ToList(), Color.green);
            }
            else
                _highlight.ClearHighlight();
        }

        if (Input.GetMouseButton(0))
        {
            Debug.LogWarning(_map.WorldPositionToXY(Utils.GetMouseWorldPosition()));

            /*if (_map.ValidateCoords(mouseCoords))
            {
                _map.PlaceMonsterOnCoords(_monster, mouseCoords);
                _monster.transform.position = _map.XYToWorldPosition(mouseCoords);
                _highlight.ClearHighlight();
            }*/
        }
    }
}
