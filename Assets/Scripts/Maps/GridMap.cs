using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;
using Project.Utils;

public struct Coords
{
    public int x;
    public int y;

    public static Coords Zero { get => new Coords(0, 0); }

    public static bool operator ==(Coords coord1, Coords coord2)
    {
        return coord1.Equals(coord2);
    }

    public static bool operator !=(Coords coord1, Coords coord2)
    {
        return !(coord1 == coord2);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is Coords)
        {
            Coords comparedCoords = (Coords)obj;
            return (x == comparedCoords.x && y == comparedCoords.y);
        }
        else
            return false;
    }

    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }

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

    public int Width { get => _width; }
    public int Height { get => _height; }


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
    public static int GetSquareSideForEntitySize(Size entitySize)
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

    public List<Coords> GetNeighboursForSingleCellEntity(Coords entityOriginCoords)
    {
        List<Coords> neighbourOffsets = new List<Coords>() { new Coords(0, -1), new Coords(-1, 0), new Coords(1, 0), new Coords(0, 1) };
        List<Coords> neighbours = new List<Coords>();

        foreach (Coords offset in neighbourOffsets)
        {
            Coords neighbour = new Coords(entityOriginCoords.x + offset.x, entityOriginCoords.y + offset.y);

            if (ValidateCoords(neighbour))
                neighbours.Add(neighbour);
        }

        return neighbours;
    }
    public List<List<Coords>> GetNeighboursForMultipleCellEntity(Coords entityOriginNode, Size entitySize)
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
    public List<Coords> GetNeighboursWithDiagonals(Coords entityOriginNode, Size entitySize)
    {
        int squareSide = GetSquareSideForEntitySize(entitySize);
        List<List<Coords>> neighboursDivided = GetNeighboursForMultipleCellEntity(entityOriginNode, entitySize);

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

            //Debug.Log($"Monster {monsterToPlace} placed on coords with origin ({targetCoords.x}, {targetCoords.y})");
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


    public List<Coords> FindPathForSingleCellEntity(Coords startCoords, Coords endCoords, bool accountForMonsters=false)
    {
        if (startCoords == endCoords)
            return null;

        GridNode endNode = GetGridObjectAtCoords(endCoords);

        if (accountForMonsters && endNode.HasImpassableObstacle || !accountForMonsters && !endNode.IsFree)
            return null;

        Queue<Coords> frontier = new Queue<Coords>();
        frontier.Enqueue(startCoords);

        Dictionary<Coords, Coords> cameFrom = new Dictionary<Coords, Coords>();
        cameFrom.Add(startCoords, new Coords(-1000, -1000));

        while (frontier.Count > 0)
        {
            Coords currentCoords = frontier.Dequeue();

            if (currentCoords == endCoords)
                break;

            List<Coords> neighbours = GetNeighboursForSingleCellEntity(currentCoords);

            foreach (Coords neighbour in neighbours)
            {
                GridNode neighbourNode = GetGridObjectAtCoords(neighbour);

                if (!cameFrom.ContainsKey(neighbour) && (accountForMonsters && !neighbourNode.HasImpassableObstacle || !accountForMonsters && neighbourNode.IsFree))
                {
                    frontier.Enqueue(neighbour);
                    cameFrom.Add(neighbour, currentCoords);
                }
            }
        }

        List<Coords> path = new List<Coords>();
        Coords currentPathCoords = endCoords;

        while (currentPathCoords != startCoords)
        {
            if (!cameFrom.ContainsKey(currentPathCoords))
                return null;

            path.Add(currentPathCoords);
            currentPathCoords = cameFrom[currentPathCoords];
        }

        if (accountForMonsters)
            path.Remove(endCoords);

        path.Reverse();
        return path;
    }
    //TODO: implement
    public List<List<Coords>> FindPathForMultipleCellEntity(Coords startCoords, Coords endCoords, Size entitySize)
    {
        /*if (startCoords == endCoords)
            return null;

        if (!GetGridObjectAtCoords(endCoords).IsFree)
            return null;

        Queue<List<Coords>> frontier = new Queue<List<Coords>>();

        List<List<Coords>> startingNeighbours = GetNeighboursWithoutDiagonals(startCoords, entitySize);
        foreach (List<Coords> neighbour in startingNeighbours)
            if (!(neighbour.All(neighbourNode => GetGridObjectAtCoords(neighbourNode).HasMonster) || neighbour.Any(neighbourNode => GetGridObjectAtCoords(neighbourNode).HasImpassableObstacle)))
                frontier.Enqueue(neighbour);
        
        Dictionary<List<Coords>, List<Coords>> cameFrom = new Dictionary<List<Coords>, List<Coords>>();
        foreach (List<Coords> neighbour in startingNeighbours)
            cameFrom.Add(neighbour, null);

        while (frontier.Count > 0)
        {
            List<Coords> currentNeighbour = frontier.Dequeue();

            if (currentNode == endNode)
                break;

            List<List<Coords>> neighbours = GetNeighbours(currentNode);

            foreach (GridNode nextNode in neighbours)
            {
                if (!cameFrom.ContainsKey(nextNode))
                {
                    frontier.Enqueue(nextNode);
                    cameFrom.Add(nextNode, currentNode);
                }
            }
        }

        List<GridNode> path = new List<GridNode>();
        GridNode currentPathNode = endNode;

        while (currentPathNode != startNode)
        {
            if (!cameFrom.ContainsKey(currentPathNode))
                return null;

            path.Add(currentPathNode);
            currentPathNode = cameFrom[currentPathNode];
        }

        path.Add(startNode);
        path.Reverse();

        return path;*/
        return null;
    }

    public List<Monster> FindMonstersInRadius(Coords entityOriginCoords, Size entitySize, int radius)
    {
        List<Monster> monsters = new List<Monster>();

        int radiusCells = radius / 5;
        //Debug.Log($"Radius cells: {radiusCells}");
        int squareSide = GetSquareSideForEntitySize(entitySize);
        int totalCells = radiusCells * 2 + squareSide;
        //Debug.Log($"Total cells: {totalCells}");

        List<Coords> entityCoords = GetListOfMonsterCoords(entityOriginCoords, entitySize);

        for (int i = -radiusCells; i < totalCells - radiusCells; i++)
            for (int j = -radiusCells; j < totalCells - radiusCells; j++)
            {
                Coords currentCell = new Coords(entityOriginCoords.x + i, entityOriginCoords.y + j);
                //Debug.Log($"Current cell: ({currentCell.x}, {currentCell.y})");

                if (!entityCoords.Contains(currentCell))
                    if (ValidateCoords(currentCell))
                    {
                        GridNode currentCellNode = GetGridObjectAtCoords(currentCell);

                        //Debug.Log($"Current cell has monster: {currentCellNode.HasMonster}");
                        if (currentCellNode.HasMonster)
                            monsters.Add(currentCellNode.Monster);
                    }
            }

        //Debug.Log($"Monsters found:");
        //foreach (Monster monster in monsters)
            //Debug.Log(monster);

        return monsters;
    }
}
