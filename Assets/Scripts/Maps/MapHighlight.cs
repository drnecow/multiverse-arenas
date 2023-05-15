using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapHighlight : MonoBehaviour
{
    private GameObject[,] _cells;
    private List<Coords> _currentlyHighlightedCells;
    private TextMeshProUGUI _currentMapText = null;
    private Dictionary<Monster, List<GameObject>> _monsterStatusIcons;


    [SerializeField] private GameObject _highlightSquarePrefab;
    [SerializeField] private GameObject _onMapTextPrefab;
    [SerializeField] private GameObject _monsterStatusIconPrefab;
    [SerializeField] private Transform _highlightParent;
    private GridMap _map;


    private void Awake()
    {
        _map = gameObject.GetComponent<GridMap>();
        _cells = new GameObject[_map.Width, _map.Height];
        _currentlyHighlightedCells = new List<Coords>();
        _monsterStatusIcons = new Dictionary<Monster, List<GameObject>>();


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
                color.a = 0.25f;
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
            color.a = 0.25f;
            square.GetComponent<SpriteRenderer>().color = color;

            _currentlyHighlightedCells.Add(cell);
        }
    }
    public void CreateMapText(Coords cell, string text)
    {
        GameObject mapTextPrefab = Instantiate(_onMapTextPrefab);
        mapTextPrefab.GetComponent<Canvas>().worldCamera = Camera.main;

        _currentMapText = mapTextPrefab.GetComponentInChildren<TextMeshProUGUI>();
        _currentMapText.text = text;
        _currentMapText.transform.position = _map.XYToWorldPosition(cell);
    }
    public void CreateMonsterStatusIcon(Monster monster, Sprite iconSprite)
    {
        GameObject statusIcon = Instantiate(_monsterStatusIconPrefab);
        statusIcon.GetComponentInChildren<Image>().sprite = iconSprite;
        statusIcon.transform.SetParent(monster.transform);
        statusIcon.transform.localPosition = new Vector3 (5, 5);

        if (_monsterStatusIcons.ContainsKey(monster))
            _monsterStatusIcons[monster].Add(statusIcon);
        else
            _monsterStatusIcons.Add(monster, new List<GameObject>() { statusIcon });
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

        foreach (Monster monster in _monsterStatusIcons.Keys)
        {
            foreach (GameObject statusIcon in _monsterStatusIcons[monster])
                Destroy(statusIcon);
        }

        if (_currentMapText != null)
        {
            Destroy(_currentMapText.gameObject.transform.parent.gameObject);
            _currentMapText = null;
        }
    }
}
