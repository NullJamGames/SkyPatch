using System;
using System.Linq;
using NJG.Runtime.PlantSystem;
using NJG.Runtime.PlotSystem;
using Sirenix.OdinInspector;

namespace NJG.Runtime.PlantSystem
{
    public static class GrowthModifierValueDropdownList 
    {
        public static ValueDropdownList<GrowthModifier> Modifiers()
        {
            var list = new ValueDropdownList<GrowthModifier>();
			
            list.Add("No Growt", new NoGrowthModifier());
            list.Add("Day Time", new DayTimeGrowtModifier());
            list.Add("Compost", new CompostGrowtModifier());
			
            return list;
        }
    }
}
