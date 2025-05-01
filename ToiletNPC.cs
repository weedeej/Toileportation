using S1API.Entities;
using ScheduleOne;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toileportation
{
    public class ToiletNPC : NPC
    {
        public ToiletNPC() : base(Guid.NewGuid().ToString(), "Golden", "Toilet", Registry.GetItem("goldentoilet").Icon)
        { }
    }
}
