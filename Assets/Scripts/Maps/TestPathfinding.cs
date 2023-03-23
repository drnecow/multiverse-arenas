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
        _map.PlaceMonsterOnCoords(_monster, Coords.Zero);
        _currentMouseCoords = _map.WorldPositionToXY(Utils.GetMouseWorldPosition());
    }

    private void Update()
    {
        Coords mouseCoords = _map.WorldPositionToXY(Utils.GetMouseWorldPosition());

        if ((mouseCoords != _currentMouseCoords) && _map.ValidateCoords(mouseCoords))
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
        }
    }
}
