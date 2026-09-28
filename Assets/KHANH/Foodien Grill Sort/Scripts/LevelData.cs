using UnityEngine;
namespace KHANH.FoodienGrillSort
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int AllFood;
        public int TotalFood;
        public int TotalGrill;
        public int LevelTime;
        public int Coin;
        public int ShipperTriggerFoodCount;
        public int RequiredFoodCount;
        public int ShipperTime;
    }
}

