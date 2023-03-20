using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;
using Project.Utils;

public struct Coords
{
    public int x;
    public int y;

    public static Coords Zero { get => new Coords(0, 0); }

    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class GridMap : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    private float _cellSize = ConstantValues.MAP_CELL_SIZE;
    [SerializeField] private Vector3 _originPosition;

    private GridNode[,] _contents;

    [SerializeField] private bool _debugViewEnabled;
    private TextMesh[,] _markupText;
    GameObject _textParent;


    private void Awake()
    {
        _contents = new GridNode[_width, _height];
        if (_debugViewEnabled)
        {
            _markupText = new TextMesh[_width, _height];
            _textParent = new GameObject("Markup Text");
        }

        // Draw vertical grid lines
        for (int i = 0; i <= _width; i++)
            Debug.DrawLine(new Vector3(i * _cellSize, 0) + _originPosition, new Vector3(i * _cellSize, -(_height * _cellSize)) + _originPosition, Color.green, 10000f);
        // Draw horizontal grid lines
        for (int j = 0; j <= _height; j++)
            Debug.DrawLine(new Vector3(0, -(j * _cellSize)) + _originPosition, new Vector3(_width * _cellSize, -(j * _cellSize)) + _originPosition, Color.green, 10000f);

        for (int x = 0; x < _contents.GetLength(0); x++)
            for (int y = 0; y < _contents.GetLength(1); y++)
                if (_debugViewEnabled)
                {
                    _contents[x, y] = new GridNode();
                    _markupText[x, y] = Utils.CreateWorldText($"{x}, {y}", null, XYToWorldPosition(new Coords(x, y)), 20, Color.black, TextAnchor.MiddleCenter);
                    _markupText[x, y].transform.SetParent(_textParent.transform);
                }
                else
                    _contents[x, y] = new GridNode();
    }


    public virtual Vector3 XYToWorldPosition(Coords XYCoords)
    {
        return new Vector3(XYCoords.x * _cellSize + _cellSize / 2, -(XYCoords.y * _cellSize + _cellSize / 2)) + _originPosition;
    }
    public virtual Coords WorldPositionToXY(Vector3 worldPosition)
    {
        Coords coords = new Coords
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize),
            y = Mathf.FloorToInt(-(worldPosition - _originPosition).y / _cellSize)
        };

        return coords;
    }
    public bool ValidateCoords(Coords coords)
    {
        if (coords.x >= 0 && coords.x < _width && coords.y >= 0 && coords.y < _height)
            return true;
        else
            return false;
    }


    public GridNode GetGridObjectAtCoords(Coords objCoords)
    {
        if (ValidateCoords(objCoords))
            return _contents[objCoords.x, objCoords.y];
        else
            return null;
    }
    public List<Coords> GetListOfMonsterCoords(Coords originCell, Size monsterSize)
    {
        int squareSizeInCells = GetSquareSideForEntitySize(monsterSize);

        List<Coords> monsterCoords = new List<Coords>();

        for (int i = 0; i < squareSizeInCells; i++)
            for (int j = 0; j < squareSizeInCells; j++)
            {
                Coords monsterCoordsCell = new Coords(originCell.x + i, originCell.y + j);
                if (!ValidateCoords(monsterCoordsCell))
                {
                    Debug.LogWarning($"Monster of size {monsterSize} cannot fit on coords with origin cell ({originCell.x}, {originCell.y})");
                    return null;
                }

                monsterCoords.Add(monsterCoordsCell);
            }

        return monsterCoords;
    }
    public int GetSquareSideForEntitySize(Size entitySize)
    {
        return entitySize switch
        {
            Size.Tiny => 1,
            Size.Small => 1,
            Size.Medium => 1,
            Size.Large => 2,
            Size.Huge => 3,
            Size.Gargantuan => 4,
            _ => 1
        };
    }
    public List<List<Coords>> GetNeighboursWithoutDiagonals(Coords entityOriginNode, Size entitySize)
    {
        int squareSide = GetSquareSideForEntitySize(entitySize);
        List<List<Coords>> neighbours = new List<List<Coords>>();

        List<Coords> topNeighbours = new List<Coords>();
        List<Coords> leftNeighbours = new List<Coords>();
        List<Coords> rightNeighbours = new List<Coords>();
        List<Coords> bottomNeighbours = new List<Coords>();

        for (int i = 0; i < squareSide; i++)
        {
            Coords topNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y - 1);
            Coords leftNeighbour = new Coords(entityOriginNode.x - 1, entityOriginNode.y + i);
            Coords bottomNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y + squareSide);
            Coords rightNeighbour = new Coords(entityOriginNode.x + squareSide, entityOriginNode.y + i);

            if (ValidateCoords(topNeighbour))
                topNeighbours.Add(topNeighbour);
            if (ValidateCoords(leftNeighbour))
                leftNeighbours.Add(leftNeighbour);
            if (ValidateCoords(bottomNeighbour))
                rightNeighbours.Add(bottomNeighbour);
            if (ValidateCoords(rightNeighbour))
                bottomNeighbours.Add(rightNeighbour);
        }

        neighbours.Add(topNeighbours);
        neighbours.Add(leftNeighbours);
        neighbours.Add(rightNeighbours);
        neighbours.Add(bottomNeighbours);

        return neighbours;
    }
    //TODO: implement
    public List<Coords> GetNeighboursWithDiagonals(Coords entityOriginNode, Size entitySize)
    {
        int squareSide = GetSquareSideForEntitySize(entitySize);
        List<List<Coords>> neighboursDivided = GetNeighboursWithoutDiagonals(entityOriginNode, entitySize);

        List<Coords> neighbours = new List<Coords>();
        foreach (List<Coords> neighbourSide in neighboursDivided)
            foreach (Coords sideCoord in neighbourSide)
                neighbours.Add(sideCoord);

        Coords topLeftNeighbour = new Coords(entityOriginNode.x - 1, entityOriginNode.y - 1);
        Coords topRightNeighbour = new Coords(entityOriginNode.x + squareSide, entityOriginNode.y - 1);
        Coords bottomLeftNeighbour = new Coords(entityOriginNode.x - 1, entityOriginNode.y + squareSide);
        Coords bottomRightNeighbour = new Coords(entityOriginNode.x + squareSide, entityOriginNode.y + squareSide);

        if (ValidateCoords(topLeftNeighbour))
            neighbours.Add(topLeftNeighbour);
        if (ValidateCoords(topRightNeighbour))
            neighbours.Add(topRightNeighbour);
        if (ValidateCoords(bottomLeftNeighbour))
            neighbours.Add(bottomLeftNeighbour);
        if (ValidateCoords(bottomRightNeighbour))
            neighbours.Add(bottomRightNeighbour);

        return neighbours;
    }


    public void PlaceMonsterOnCoords(Monster monsterToPlace, Coords targetCoords)
    {
        List<Coords> newCoords = GetListOfMonsterCoords(targetCoords, monsterToPlace.Stats.Size);

        if (newCoords != null)
        {
            List<Coords> oldCoords = monsterToPlace.CurrentCoords;

            foreach (Coords oldCoord in oldCoords)
                GetGridObjectAtCoords(oldCoord).Monster = null;

            foreach (Coords newCoord in newCoords)
                GetGridObjectAtCoords(newCoord).Monster = monsterToPlace;
        }
        else
        {
            Debug.LogWarning($"Cannot place monster {monsterToPlace} on coords ({targetCoords.x}, {targetCoords.y})");
        }
    }
    //TODO: implement
    public void PlaceObstacleOnCoords(Obstacle obstacleToPlace, Coords targetCoords)
    {

    }


    //TODO: implement
    public List<GridNode> FindPath(Coords startCoords, Coords endCoords, Size entitySize)
    {
        return null;
    }
}
