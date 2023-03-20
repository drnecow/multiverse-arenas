using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Constants;
using Project.Dice;

public class Monster : MonoBehaviour
{
    [SerializeField] private GridMap _grid;
    public GridMap Grid { get => _grid; set => _grid = value; }

    [field: SerializeField] public MonsterStats Stats { get; private set; }
    [SerializeField] private int _numberOfAttacks;

    [SerializeField] private List<CombatAction> _freeActions;
    [SerializeField] private List<CombatAction> _mainActions;
    [SerializeField] private List<CombatAction> _bonusActions;

    [SerializeField] private MeleeAttackIntDictionary _meleeAttacks;
    [SerializeField] private RangedAttackIntDictionary _rangedAttacks;

    public HashSet<Condition> ActiveConditions { get; private set; }

    public Coords CurrentCoordsOriginCell //returns top left square of monster's position; the rest of the squares are deduced through monster's Size
    {
        get
        {
            float cellSize = ConstantValues.MAP_CELL_SIZE;

            return Stats.Size switch
            {
                Size.Tiny => _grid.WorldPositionToXY(transform.position),
                Size.Small => _grid.WorldPositionToXY(transform.position),
                Size.Medium => _grid.WorldPositionToXY(transform.position),
                Size.Large => _grid.WorldPositionToXY(new Vector3(transform.position.x - cellSize/2, transform.position.y + cellSize/2)),
                Size.Huge => _grid.WorldPositionToXY(new Vector3(transform.position.x - cellSize, transform.position.y + cellSize)),
                Size.Gargantuan => _grid.WorldPositionToXY(new Vector3(transform.position.x - (cellSize + cellSize/2), transform.position.y + (cellSize + cellSize/2))),
                _ => _grid.WorldPositionToXY(transform.position)
            };
        }
    }
    public List<Coords> CurrentCoords
    {
        get => _grid.GetListOfMonsterCoords(CurrentCoordsOriginCell, Stats.Size);
    }
    [ContextMenu("Get current coordinates")]
    public void GetCurrentCoords()
    {
        List<Coords> currentCoords = CurrentCoords;
        string output = "";

        foreach (Coords coords in currentCoords)
        {
            output += $"({coords.x}, {coords.y}) ";
        }

        Debug.Log(output);
    }
    [ContextMenu("Get neighbours with diagonals")]
    public void GetNeighbours()
    {
        List<Coords> neighbours = _grid.GetNeighboursWithDiagonals(CurrentCoordsOriginCell, Stats.Size);
        string output = "";

        foreach (Coords coords in neighbours)
        {
            output += $"({coords.x}, {coords.y}) ";
        }

        Debug.Log(output);
    }

    private bool _mainActionUsed = false;
    public bool MainActionUsed { get => _mainActionUsed; set => _mainActionUsed = value; }
    private bool _bonusActionUsed = false;
    public bool BonusActionUsed { get => _bonusActionUsed; set => _bonusActionUsed = value; }
    private bool _reactionUsed = false;
    public bool ReactionUsed { get => _reactionUsed; set => _reactionUsed = value; }

    public void AddActiveCondition(Condition condition)
    {
        ActiveConditions.Add(condition);
    }


    private void Awake()
    {
        ActiveConditions = new HashSet<Condition>();
    }

    public void TakeDamage(int damagePoints, DamageType damageType)
    {
        if (Stats.DamageImmunities.Contains(damageType))
        {
            Debug.Log($"{Stats.Name} is immune to {damageType} damage, so no damage to it");
        }
        else if (Stats.DamageResistances.Contains(damageType))
        {
            Debug.Log($"{Stats.Name} is resistant to {damageType} damage, so half damage to it: {damagePoints / 2}");
            Stats.CurrentHP -= damagePoints / 2;
        }
        else if (Stats.DamageVulnerabilities.Contains(damageType))
        {
            Debug.Log($"{Stats.Name} is vulnerable to {damageType} damage, so double damage to it: {damagePoints * 2}");
            Stats.CurrentHP -= damagePoints * 2;
        }
        else
        {
            Debug.Log($"{Stats.Name} takes {damagePoints} points of {damageType} damage");
            Stats.CurrentHP -= damagePoints;
        }
    }

    //TODO: implement this method
    public RollMode ResolveAdvantageAndDisadvantageToMeleeAttack(Monster target)
    {
        return RollMode.Normal;
    }
    //TODO: implement this method
    public RollMode ResolveAdvantageAndDisadvantageToRangedAttack(Monster target, int feetToTarget)
    {
        return RollMode.Normal;
    }
}
