using S1API.Entities;
using ScheduleOne;

namespace Toileportation
{
    public class ToiletNPC : NPC
    {
        public ToiletNPC() : base(Guid.NewGuid().ToString(), "Golden", "Toilet", Registry.GetItem("goldentoilet").Icon)
        { }
    }
}
