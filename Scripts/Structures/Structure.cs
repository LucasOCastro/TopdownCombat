using Godot;

namespace CombatGame
{
    public class Structure : MapObjectInstance<StructureBase>
    {
        public Structure(StructureBase structureBase) : base(structureBase)
        {
        }
    }
}