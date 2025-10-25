using IdleTycoon.Scripts.CustomEditor.Attributes;
using IdleTycoon.Scripts.Data.Enums;
using UnityEngine;

namespace IdleTycoon.Scripts.Data.Definitions
{
    [CreateAssetMenu(fileName = "TileStructure", menuName = "Definitions/Data/TileStructure", order = 0)]
    public class TileStructureDefinition : ScriptableObject
    {
        [SerializeField] protected int id;
        
        [SerializeField, EnumFlags] protected TileAttributeFlag placementTileConditionsRequired;
        
        [SerializeField, EnumFlags] protected TileAttributeFlag placementTileConditionsCollision;
        
        
    }
}