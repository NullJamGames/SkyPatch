using Sirenix.OdinInspector;

namespace NJG.Runtime.PlantSystem
{
    public static class PlotInteractionValueDropdownList
    {
        public static ValueDropdownList<PlotInteraction> Interactions()
        {
            var list = new ValueDropdownList<PlotInteraction>();
			
            list.Add("Change State", new ChangeStateInteraction());
            list.Add("Place Seed", new PlaceSeedInteraction());
            list.Add("Harvest", new HarvestInteraction());
			
            return list;
        }
    }
}