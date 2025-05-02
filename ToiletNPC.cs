using S1API.Entities;
using Il2CppScheduleOne;

namespace Toileportation
{
    public class ToiletNPC : NPC
    {
        public ToiletNPC() : base(Guid.NewGuid().ToString(), "Golden", "Toilet", Registry.GetItem("goldentoilet").Icon)
        { }
    }
}
