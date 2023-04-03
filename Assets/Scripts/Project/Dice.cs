using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Dice
{
    public enum Die
    {
        D4 = 4,
        D6 = 6,
        D8 = 8,
        D10 = 10,
        D12 = 12,
        D20 = 20,
        D100 = 100
    }

    public enum RollMode
    {
        Normal = 1,
        Advantage = 2,
        Disadvantage = 3
    }

    public static class Dice
    {
        public static int RollD4(int numRolls=1, RollMode rollMode=RollMode.Normal)
        {
            return RollDice(Die.D4, numRolls, rollMode);
        }

        public static int RollD6(int numRolls=1, RollMode rollMode=RollMode.Normal)
        {
            return RollDice(Die.D6, numRolls, rollMode);
        }

        public static int RollD8(int numRolls=1, RollMode rollMode=RollMode.Normal)
        {
            return RollDice(Die.D8, numRolls, rollMode);
        }

        public static int RollD10(int numRolls=1, RollMode rollMode=RollMode.Normal)
        {
            return RollDice(Die.D10, numRolls, rollMode);
        }

        public static int RollD12(int numRolls=1, RollMode rollMode=RollMode.Normal)
        {
            return RollDice(Die.D12, numRolls, rollMode);
        }

        public static int RollD20(int numRolls=1, RollMode rollMode=RollMode.Normal)
        {
            return RollDice(Die.D20, numRolls, rollMode);
        }

        public static int RollD100(int numRolls=1, RollMode rollMode=RollMode.Normal)
        {
            return RollDice(Die.D100, numRolls, rollMode);
        }

        // Roll numRolls dice with "die" side in either Normal, Advantage, or Disadvantage mode determined by rollMode
        public static int RollDice(Die die, int numRolls, RollMode rollMode=RollMode.Normal)
        {
            int dieSide = (int)die;

            int rollSum1 = 0;
            string rolledNumbers1 = "";

            int roll;

            for (int i = 0; i < numRolls; i++)
            {
                roll = Random.Range(1, dieSide + 1);
                rollSum1 += roll;
                rolledNumbers1 += $"{roll} ";
            }

            if (rollMode == RollMode.Advantage || rollMode == RollMode.Disadvantage)
            {
                int rollSum2 = 0;
                string rolledNumbers2 = "";

                for (int i = 0; i < numRolls; i++)
                {
                    roll = Random.Range(1, dieSide + 1);
                    rollSum2 += roll;
                    rolledNumbers2 += $"{roll} ";
                }

                //Debug.Log($"First roll: {rolledNumbers1}");
                //Debug.Log($"Second roll: {rolledNumbers2}");

                if (rollMode == RollMode.Advantage)
                    return Mathf.Max(rollSum1, rollSum2);
                else
                    return Mathf.Min(rollSum1, rollSum2);
            }

            //Debug.Log(rolledNumbers1);

            return rollSum1;
        }
    }
}