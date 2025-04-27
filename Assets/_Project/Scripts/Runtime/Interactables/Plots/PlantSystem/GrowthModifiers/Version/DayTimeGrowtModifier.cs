using System;
using NJG.Runtime.Interactables;
using NJG.Runtime.PlotSystem;
using UnityEngine;

namespace NJG.Runtime.PlantSystem
{
    public class DayTimeGrowtModifier : GrowthModifier
    {
        [SerializeField] private EGrowtDayTime _growableDayTime;
        
        public override float CalculateGrowth(OldPlot oldPlot)
        {
            switch (_growableDayTime)
            {
                case EGrowtDayTime.day:
                    if(oldPlot.IsDaytime);
                    return 1;
                    return 0;
                case EGrowtDayTime.night:
                    if(!oldPlot.IsDaytime);
                    return 1;
                    return 0;
            }

            return 1;
        }
        
        public override string ToString()
        {
            return "Day Time";
        }
    }
}
