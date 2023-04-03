using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHighlight : MonoBehaviour
{
    GameObject[,] _cells;
    List<Coords> _currentlyHighlightedCells;

    [SerializeField] private GameObject _highlightSquarePrefab;
    [SerializeField] private Transform _highlightParent;
    private GridMap _map;


    private void Awake()
    {
        _map = gameObject.GetComponent<GridMap>();
        _cells = new GameObject[_map.Width, _map.Height];
        _currentlyHighlightedCells = new List<Coords>();


        for (int i = 0; i < _cells.GetLength(0); i++)
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                GameObject square = Instantiate(_highlightSquarePrefab);
                square.transform.position = _map.XYToWorldPosition(new Coords(i, j));
                square.transform.SetParent(_highlightParent);

                _cells[i, j] = square;
            }
    }

    public void HighlightCells(List<Coords> cells, Color cellColor)
    {
        foreach (Coords cell in cells)
            if (_map.ValidateCoords(cell))
            {
                GameObject square = _cells[cell.x, cell.y];

                Color color = cellColor;
                color.a = 1f;
                square.GetComponent<SpriteRenderer>().color = color;

                _currentlyHighlightedCells.Add(cell);
            }
    }
    public void HighlightCell(Coords cell, Color cellColor)
    {
        if (_map.ValidateCoords(cell))
        {
            GameObject square = _cells[cell.x, cell.y];

            Color color = cellColor;
            color.a = 1f;
            square.GetComponent<SpriteRenderer>().color = color;

            _currentlyHighlightedCells.Add(cell);
        }
    }
    public void ClearHighlight()
    {
        foreach (Coords cell in _currentlyHighlightedCells)
        {
            GameObject square = _cells[cell.x, cell.y];

            Color color = square.GetComponent<SpriteRenderer>().color;
            color.a = 0f;
            square.GetComponent<SpriteRenderer>().color = color;
        }

        _currentlyHighlightedCells.Clear();
    }
}
