using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Utils;

public class TestPathfinding : MonoBehaviour
{
    [SerializeField] private MapHighlight _highlight;
    [SerializeField] private GridMap _map;

    [SerializeField] private Monster _monster;
    private Coords _currentMouseCoords;


    private void Start()
    {
        _currentMouseCoords = _map.WorldPositionToXY(Utils.GetMouseWorldPosition());
        List<List<Coords>> path = _map.FindPathToMonsterForMultipleCellEntity(_monster, _monster.VisibleTargets[0]);

        if (path != null)
        {
            _highlight.ClearHighlight();
            _highlight.HighlightCells(_monster.CurrentCoords, Color.green);

            foreach (List<Coords> pathLine in path)
                _highlight.HighlightCells(pathLine, Color.green);
        }
    }

    /*private void Update()
    {
        //if (Input.GetMouseButton(0))
        //{

            Coords mouseCoords = _map.WorldPositionToXY(Utils.GetMouseWorldPosition());

            *//*if ((mouseCoords != _currentMouseCoords) && _map.ValidateCoords(mouseCoords))
            {
                List<Coords> path = _map.FindPathForSingleCellEntity(_monster.CurrentCoordsOriginCell, mouseCoords);

                if (path != null)
                {
                    _highlight.ClearHighlight();
                    _highlight.HighlightCell(_monster.CurrentCoordsOriginCell);
                    _highlight.HighlightCells(path);
                }

                _currentMouseCoords = mouseCoords;
            }

            if (Input.GetMouseButton(0))
            {
                if (_map.ValidateCoords(mouseCoords))
                {
                    _map.PlaceMonsterOnCoords(_monster, mouseCoords);
                    _monster.transform.position = _map.XYToWorldPosition(mouseCoords);
                    _highlight.ClearHighlight();
                }
            }*//*

            if ((mouseCoords != _currentMouseCoords) && _map.ValidateCoords(mouseCoords))
            {
            List<List<Coords>> path = _map.FindPathToFreeCellForMultipleCellEntity(_monster, _monster.CurrentCoordsOriginCell, mouseCoords, _monster.Stats.Size);
            //List<List<Coords>> path = _map.FindPathToMonsterForMultipleCellEntity(_monster, _monster.VisibleTargets[0]);

            if (path != null)
                {
                    _highlight.ClearHighlight();
                    _highlight.HighlightCells(_monster.CurrentCoords);

                    foreach (List<Coords> pathLine in path)
                        _highlight.HighlightCells(pathLine);
                }
                else
                    Debug.Log("No path");

                _currentMouseCoords = mouseCoords;
            }

        //}
    }*/
}
