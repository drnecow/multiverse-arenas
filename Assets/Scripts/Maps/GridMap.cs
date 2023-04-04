using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Constants;
using Project.Utils;

public struct Coords
{
    public int x { get; private set; }
    public int y { get; private set; }

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

    public override string ToString()
    {
        return $"({x}, {y})";
    }

    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public enum Direction
{
    Top = 1, 
    Left = 2,
    Right = 3,
    Bottom = 4,

    TopLeft = 5,
    TopRight = 6,
    BottomLeft = 7,
    BottomRight = 8
}
public struct MultipleCellLine
{
    public List<Coords> Coords { get; private set; }
    public Direction? RelativeDirection { get; private set; }

    public MultipleCellLine(List<Coords> coords, Direction? direction)
    {
        Coords = coords;
        RelativeDirection = direction;
    }

    public static MultipleCellLine Null = new MultipleCellLine(new List<Coords>(), null);

    public static bool operator ==(MultipleCellLine line1, MultipleCellLine line2)
    {
        return line1.Equals(line2);
    }

    public static bool operator !=(MultipleCellLine line1, MultipleCellLine line2)
    {
        return !(line1 == line2);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is MultipleCellLine)
        {
            MultipleCellLine comparedLine = (MultipleCellLine)obj;
            return (Coords.SequenceEqual(comparedLine.Coords) && RelativeDirection == comparedLine.RelativeDirection);
        }
        else
            return false;
    }

    public override int GetHashCode()
    {
        return (String.Join(" ", Coords), RelativeDirection).GetHashCode();
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
    private TextMesh[,] _cellOccupationIndicatingText;
    GameObject _textParent;

    public int Width { get => _width; }
    public int Height { get => _height; }


    private void Awake()
    {
        _contents = new GridNode[_width, _height];
        if (_debugViewEnabled)
        {
            _markupText = new TextMesh[_width, _height];
            _cellOccupationIndicatingText = new TextMesh[_width, _height];
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

                    _cellOccupationIndicatingText[x, y] = Utils.CreateWorldText("True", null, XYToWorldPosition(new Coords(x, y)), 20, Color.green, TextAnchor.MiddleCenter);
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
        Coords coords = new Coords(Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize), Mathf.FloorToInt(-(worldPosition - _originPosition).y / _cellSize));
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
    public static bool IsSingleCelledSize(Size monsterSize)
    {
        if (monsterSize == Size.Tiny || monsterSize == Size.Small || monsterSize == Size.Medium)
            return true;
        else
            return false;
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
    public List<MultipleCellLine> GetNeighboursForMultipleCellEntity(Coords entityOriginNode, int entitySquareSide, bool includeDiagonals=false)
    {
        List<MultipleCellLine> neighbours = new List<MultipleCellLine>();

        List<Coords> topNeighbours = new List<Coords>();
        List<Coords> leftNeighbours = new List<Coords>();
        List<Coords> rightNeighbours = new List<Coords>();
        List<Coords> bottomNeighbours = new List<Coords>();

        for (int i = 0; i < entitySquareSide; i++)
        {
            Coords topNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y - 1);
            Coords leftNeighbour = new Coords(entityOriginNode.x - 1, entityOriginNode.y + i);
            Coords bottomNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y + entitySquareSide);
            Coords rightNeighbour = new Coords(entityOriginNode.x + entitySquareSide, entityOriginNode.y + i);

            if (ValidateCoords(topNeighbour))
                topNeighbours.Add(topNeighbour);
            if (ValidateCoords(leftNeighbour))
                leftNeighbours.Add(leftNeighbour);
            if (ValidateCoords(rightNeighbour))
                rightNeighbours.Add(rightNeighbour);
            if (ValidateCoords(bottomNeighbour))
                bottomNeighbours.Add(bottomNeighbour);
        }

        if (topNeighbours.Count == entitySquareSide)
        {
            MultipleCellLine topLine = new MultipleCellLine(topNeighbours, Direction.Top);
            neighbours.Add(topLine);
        }
        if (leftNeighbours.Count == entitySquareSide)
        {
            MultipleCellLine leftLine = new MultipleCellLine(leftNeighbours, Direction.Left);
            neighbours.Add(leftLine);
        }
        if (rightNeighbours.Count == entitySquareSide)
        {
            MultipleCellLine rightLine = new MultipleCellLine(rightNeighbours, Direction.Right);
            neighbours.Add(rightLine);
        }
        if (bottomNeighbours.Count == entitySquareSide)
        {
            MultipleCellLine bottomLine = new MultipleCellLine(bottomNeighbours, Direction.Bottom);
            neighbours.Add(bottomLine);
        }

        if (includeDiagonals)
        {
            List<Coords> topLeftNeighbours = new List<Coords>();
            List<Coords> topRightNeighbours = new List<Coords>();
            List<Coords> bottomLeftNeighbours = new List<Coords>();
            List<Coords> bottomRightNeighbours = new List<Coords>();

            for (int i = -entitySquareSide; i < 0; i++)
            {
                Coords topLeftNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y - 1);
                Coords bottomLeftNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y + entitySquareSide);

                if (ValidateCoords(topLeftNeighbour))
                    topLeftNeighbours.Add(topLeftNeighbour);
                if (ValidateCoords(bottomLeftNeighbour))
                    bottomLeftNeighbours.Add(bottomLeftNeighbour);
            }
            for (int i = entitySquareSide; i < entitySquareSide * 2 - 1; i++)
            {
                Coords topRightNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y - 1);
                Coords bottomRightNeighbour = new Coords(entityOriginNode.x + i, entityOriginNode.y + entitySquareSide);

                if (ValidateCoords(topRightNeighbour))
                    topRightNeighbours.Add(topRightNeighbour);
                if (ValidateCoords(bottomRightNeighbour))
                    bottomRightNeighbours.Add(bottomRightNeighbour);
            }

            if (topLeftNeighbours.Count == entitySquareSide)
            {
                MultipleCellLine topLeftLine = new MultipleCellLine(topLeftNeighbours, Direction.TopLeft);
                neighbours.Add(topLeftLine);
            }
            if (topRightNeighbours.Count == entitySquareSide)
            {
                MultipleCellLine topRightLine = new MultipleCellLine(topRightNeighbours, Direction.TopRight);
                neighbours.Add(topRightLine);
            }
            if (bottomLeftNeighbours.Count == entitySquareSide)
            {
                MultipleCellLine bottomLeftLine = new MultipleCellLine(bottomLeftNeighbours, Direction.BottomLeft);
                neighbours.Add(bottomLeftLine);
            }
            if (bottomRightNeighbours.Count == entitySquareSide)
            {
                MultipleCellLine bottomRightLine = new MultipleCellLine(bottomRightNeighbours, Direction.BottomRight);
                neighbours.Add(bottomRightLine);
            }
        }

        return neighbours;
    }


    public void PlaceMonsterOnCoords(Monster monsterToPlace, Coords targetCoords)
    {
        List<Coords> newCoords = GetListOfMonsterCoords(targetCoords, monsterToPlace.Stats.Size);

        if (newCoords != null)
        {
            foreach (Coords newCoord in newCoords)
            {
                GetGridObjectAtCoords(newCoord).Monster = monsterToPlace;

                if (_debugViewEnabled)
                {
                    TextMesh cellText = _cellOccupationIndicatingText[newCoord.x, newCoord.y];
                    cellText.text = "False";
                    cellText.color = Color.red;
                }
            }

            //Debug.Log($"Monster {monsterToPlace} placed on coords with origin ({targetCoords.x}, {targetCoords.y})");
        }
        else
        {
            Debug.LogWarning($"Cannot place monster {monsterToPlace} on coords ({targetCoords.x}, {targetCoords.y})");
        }
    }
    public void FreeCurrentCoordsOfMonster(Monster monster)
    {
        List<Coords> freedCoords = monster.CurrentCoords;

        foreach (Coords freedCoord in freedCoords)
        {
            GetGridObjectAtCoords(freedCoord).Monster = null;

            if (_debugViewEnabled)
            {
                TextMesh cellText = _cellOccupationIndicatingText[freedCoord.x, freedCoord.y];
                cellText.text = "True";
                cellText.color = Color.green;
            }
        }
    }
    //TODO: implement
    public void PlaceObstacleOnCoords(Obstacle obstacleToPlace, Coords targetCoords)
    {

    }


    public List<Coords> FindPathForSingleCellEntity(Coords startCoords, Coords endCoords, Monster targetMonster=null)
    {
        if (!ValidateCoords(endCoords))
            return null;

        if (startCoords == endCoords)
            return null;

        GridNode endNode = GetGridObjectAtCoords(endCoords);

        if (targetMonster != null && endNode.HasImpassableObstacle || targetMonster == null && !endNode.IsFree)
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

            List<Coords> neighbours = GetValidSingleCellNeighbours(GetNeighboursForSingleCellEntity(currentCoords), targetMonster);

            foreach (Coords neighbour in neighbours)
                if (!cameFrom.ContainsKey(neighbour))
                {
                    frontier.Enqueue(neighbour);
                    cameFrom.Add(neighbour, currentCoords);
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

        if (targetMonster != null)
            path.Remove(endCoords);

        path.Reverse();
        return path;
    }
    // TODO: improve pathfinding
    public List<List<Coords>> FindPathToFreeCellForMultipleCellEntity(Monster entity, Coords entityOriginCoords, Coords endCoords, Size entitySize)
    {
        if (entityOriginCoords == endCoords)
        {
            Debug.LogWarning("Trying to step on the same cell");
            return null;
        }

        int squareSide = GetSquareSideForEntitySize(entitySize);

        List<Coords> endCoordsSquare = GetListOfMonsterCoords(endCoords, entitySize);
        if (endCoordsSquare == null)
        {
            Debug.LogWarning("Cannot fit");
            return null;
        }

        if (!endCoordsSquare.All((coord) => GetGridObjectAtCoords(coord).IsFree))
        {
            foreach (Coords coord in endCoordsSquare)
            {
                GridNode coordNode = GetGridObjectAtCoords(coord);
                if (coordNode.HasMonster && coordNode.Monster != entity)
                {
                    Debug.LogWarning("Some of the square coordinates aren't free");

                    foreach (Coords squareCoords in endCoordsSquare)
                    {
                        GridNode node = GetGridObjectAtCoords(squareCoords);
                        Debug.Log(squareCoords.ToString() + ": " + node.IsFree);
                        Debug.Log($"Obstacle: {node.Obstacle}");
                        Debug.Log($"Monster: {node.Monster}");
                    }

                    return null;
                }
            }
        }

        List<MultipleCellLine> initialNeighbours = GetValidMultipleCellNeighbours(GetNeighboursForMultipleCellEntity(entityOriginCoords, squareSide));

        foreach (MultipleCellLine neighbour in initialNeighbours)
        {
            Debug.Log($"Neighbour {neighbour.RelativeDirection}:");

            foreach (Coords coord in neighbour.Coords)
            {
                Debug.Log(coord.ToString());
            }
        }

        Queue<MultipleCellLine> frontier = new Queue<MultipleCellLine>();
        Dictionary<MultipleCellLine, MultipleCellLine> cameFrom = new Dictionary<MultipleCellLine, MultipleCellLine>();

        foreach (MultipleCellLine neighbourLine in initialNeighbours) {
            frontier.Enqueue(neighbourLine);
            cameFrom.Add(neighbourLine, MultipleCellLine.Null);
        }

        List<Coords> verticalEndLineCoords = GetFirstLineOfCell(endCoords, squareSide, Direction.Bottom);
        List<Coords> horizontalEndLineCoords = GetFirstLineOfCell(endCoords, squareSide, Direction.Right);

        Debug.Log("Vertical destination line:");
        foreach (Coords coord in verticalEndLineCoords)
        {
            Debug.Log(coord.ToString());
        }
        Debug.Log("Horizontal destination line:");
        foreach (Coords coord in horizontalEndLineCoords)
        {
            Debug.Log(coord.ToString());
        }

        MultipleCellLine[] possibleEndLines =
        {
            new MultipleCellLine(verticalEndLineCoords, Direction.Left),
            new MultipleCellLine(verticalEndLineCoords, Direction.Right),
            new MultipleCellLine(horizontalEndLineCoords, Direction.Top),
            new MultipleCellLine(horizontalEndLineCoords, Direction.Bottom)
        };

        while (frontier.Count > 0)
        {
            MultipleCellLine currentLine = frontier.Dequeue();

            if (possibleEndLines.Contains(currentLine))
                break;

            string currLine = "";
            foreach (Coords coord in currentLine.Coords)
                currLine += coord.ToString() + " ";
            //Debug.Log("Currently explored line: " + currLine);

            Coords currentLineOriginCoords = currentLine.RelativeDirection switch
            {
                Direction.Top => currentLine.Coords[0],
                Direction.Left => currentLine.Coords[0],
                Direction.Right => new Coords(currentLine.Coords[0].x - (squareSide - 1), currentLine.Coords[0].y),
                Direction.Bottom => new Coords(currentLine.Coords[0].x, currentLine.Coords[0].y - (squareSide - 1)),
                _ => currentLine.Coords[0]
            };
            //Debug.Log($"Direction of currently explored line: {currentLine.RelativeDirection}");
            //Debug.Log("Origin coords of a square of currently explored line: " + currentLineOriginCoords.ToString());

            List<MultipleCellLine> neighbours = GetValidMultipleCellNeighbours(GetNeighboursForMultipleCellEntity(currentLineOriginCoords, squareSide));

            foreach (MultipleCellLine neighbour in neighbours)
            {
                if (!cameFrom.ContainsKey(neighbour))
                {
                    frontier.Enqueue(neighbour);
                    cameFrom.Add(neighbour, currentLine);
                }
            }
        }

        List<List<MultipleCellLine>> paths = new List<List<MultipleCellLine>>();

        foreach (MultipleCellLine possibleEndLine in possibleEndLines)
        {
            List<MultipleCellLine> path = new List<MultipleCellLine>();
            MultipleCellLine currentCheckedLine = possibleEndLine;

            while (!initialNeighbours.Contains(currentCheckedLine))
            {
                if (!cameFrom.ContainsKey(currentCheckedLine))
                {
                    path = null;
                    break;
                }

                path.Add(currentCheckedLine);
                currentCheckedLine = cameFrom[currentCheckedLine];
            }

            if (initialNeighbours.Contains(currentCheckedLine))
                path.Add(currentCheckedLine);

            paths.Add(path);
        }

        paths = paths.Where(path => path != null).ToList();
        if (paths.Count == 0)
        {
            Debug.Log("No path");
            return null;
        }

        List<MultipleCellLine> shortestPath = paths[0];

        foreach (List<MultipleCellLine> path in paths)
            if (path.Count < shortestPath.Count)
                shortestPath = path;

        List<List<Coords>> finalPath = shortestPath.ConvertAll((pathLine) => pathLine.Coords);
        Debug.Log(finalPath.Count);
        finalPath.Reverse();

        return finalPath;
    }
    // TODO: fix and finish
    public List<List<Coords>> FindPathToMonsterForMultipleCellEntity(Monster entity, Monster monster)
    {
        int squareSide = GetSquareSideForEntitySize(entity.Stats.Size);
        Coords entityOriginCoords = entity.CurrentCoordsOriginCell;

        List<MultipleCellLine> possibleApproachPositions = GetNeighboursForMultipleCellEntity(monster.CurrentCoordsOriginCell, GetSquareSideForEntitySize(monster.Stats.Size), true);
        List<List<List<Coords>>> paths = new List<List<List<Coords>>>();

        foreach (MultipleCellLine approachPosition in possibleApproachPositions)
        {
            Coords positionOriginCell = approachPosition.RelativeDirection switch
            {
                Direction.Top => new Coords(approachPosition.Coords[0].x, approachPosition.Coords[0].y - (squareSide - 1)),
                Direction.Left => new Coords(approachPosition.Coords[0].x - (squareSide - 1), approachPosition.Coords[0].y),
                Direction.Right => approachPosition.Coords[0],
                Direction.Bottom => approachPosition.Coords[0],

                Direction.TopLeft => new Coords(approachPosition.Coords[0].x, approachPosition.Coords[0].y - (squareSide - 1)),
                Direction.TopRight => new Coords(approachPosition.Coords[0].x, approachPosition.Coords[0].y - (squareSide - 1)),
                Direction.BottomLeft => approachPosition.Coords[0],
                Direction.BottomRight => approachPosition.Coords[0],
                _ => approachPosition.Coords[0]
            };

            if (ValidateCoords(positionOriginCell))
            {
                List<List<Coords>> positionPath = FindPathToFreeCellForMultipleCellEntity(entity, entityOriginCoords, positionOriginCell, entity.Stats.Size);

                if (positionPath != null)
                    paths.Add(positionPath);
            }
        }

        if (paths.Count == 0)
        {
            Debug.Log("No path");
            return null;
        }

        List<List<Coords>> shortestPath = paths[0];

        foreach (List<List<Coords>> path in paths)
            if (path.Count < shortestPath.Count)
                shortestPath = path;

        return shortestPath;
    }
    private List<Coords> GetFirstLineOfCell(Coords originCell, int squareSide, Direction directionFromCell)
    {
        List<Coords> line = new List<Coords>();

        if (directionFromCell == Direction.Top)
        {
            for (int i = 0; i > -squareSide; i--)
            {
                Coords lineCell = new Coords(originCell.x, originCell.y + i);

                if (ValidateCoords(lineCell))
                    line.Add(lineCell);
            }
        }
        else if (directionFromCell == Direction.Bottom)
        {
            for (int i = 0; i < squareSide; i++)
            {
                Coords lineCell = new Coords(originCell.x, originCell.y + i);

                if (ValidateCoords(lineCell))
                    line.Add(lineCell);
            }
        }
        else if (directionFromCell == Direction.Left)
        {
            for (int i = 0; i > -squareSide; i--)
            {
                Coords lineCell = new Coords(originCell.x + i, originCell.y);

                if (ValidateCoords(lineCell))
                    line.Add(lineCell);
            }
        }
        else if (directionFromCell == Direction.Right)
        {
            for (int i = 0; i < squareSide; i++)
            {
                Coords lineCell = new Coords(originCell.x + i, originCell.y);

                if (ValidateCoords(lineCell))
                    line.Add(lineCell);
            }
        }

        return line;
    } 

    private List<Coords> GetValidSingleCellNeighbours(List<Coords> neighbours, Monster targetMonster)
    {
        List<Coords> validNeighbours = new List<Coords>();

        foreach (Coords neighbour in neighbours)
        {
            GridNode neighbourNode = GetGridObjectAtCoords(neighbour);

            if (targetMonster == null)
            {
                if (neighbourNode.IsFree)
                    validNeighbours.Add(neighbour);
            }
            else
            {
                if (neighbourNode.IsFree || neighbourNode.Monster == targetMonster)
                    validNeighbours.Add(neighbour);
            }
        }

        return validNeighbours;
    }
    private List<MultipleCellLine> GetValidMultipleCellNeighbours(List<MultipleCellLine> neighbourLines)
    {
        List<MultipleCellLine> validNeighbours = new List<MultipleCellLine>();

        foreach (MultipleCellLine neighbourLine in neighbourLines)
        {
            if (!neighbourLine.Coords.Any(coord => GetGridObjectAtCoords(coord).HasImpassableObstacle) &&
                !neighbourLine.Coords.TrueForAll(coord => GetGridObjectAtCoords(coord).HasMonster))
                    validNeighbours.Add(neighbourLine);
        }

        return validNeighbours;
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
