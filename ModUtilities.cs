using ScheduleOne.EntityFramework;
using ScheduleOne.ObjectScripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toileportation
{
    public static class ModUtilities
    {
        public static Dictionary<string, BuildableItem[]> GetAllGoldenToilet()
        {
            BuildableItem[] goldenToilets = GameObject.FindObjectsOfType<BuildableItem>().Where((builtItem) => builtItem.ParentProperty.IsOwned && builtItem.ItemInstance.Name == "Golden Toilet").ToArray();
            Dictionary<string, BuildableItem[]> toiletDict = new Dictionary<string, BuildableItem[]>();
            foreach (var toilet in goldenToilets)
            {
                string propertyName = toilet.ParentProperty.PropertyName;
                if (!toiletDict.ContainsKey(propertyName))
                {
                    toiletDict[propertyName] = new BuildableItem[] { toilet };
                }
                else
                {
                    List<BuildableItem> items = new List<BuildableItem>(toiletDict[propertyName]);
                    items.Add(toilet);
                    toiletDict[propertyName] = items.ToArray();
                }
            }

            return toiletDict;
        }
    }
}
