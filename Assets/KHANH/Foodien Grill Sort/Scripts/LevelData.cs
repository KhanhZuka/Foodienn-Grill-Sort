using UnityEngine;
namespace KHANH.FoodienGrillSort
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int allFood;
        public int totalFood;
        public int totalGrill;
        public int levelTime;
    }
}

