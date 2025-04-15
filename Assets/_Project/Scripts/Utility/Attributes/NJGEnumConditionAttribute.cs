using UnityEngine;
using System;
using System.Collections;

namespace NJG.Utilities
{
    /// <summary>
    /// An attribute to conditionally hide fields based on the current selection in an enum.
    /// Usage :  [NJGEnumCondition("resourceType", false, (int)ResourceType.Food, (int)ResourceType.Water)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class NJGEnumConditionAttribute : PropertyAttribute
    {
        public string ConditionEnum = "";
        public bool Hidden = false;
        public bool Inverse = false;

        BitArray bitArray = new BitArray(32);
        public bool ContainsBitFlag(int enumValue)
        {
            return bitArray.Get(enumValue) ^ Inverse; // Apply inverse logic
        }

        public NJGEnumConditionAttribute(string conditionEnum, bool inverse, params int[] enumValues)
        {
            this.ConditionEnum = conditionEnum;
            this.Hidden = true;
            this.Inverse = inverse;

            for (int i = 0; i < enumValues.Length; i++)
            {
                bitArray.Set(enumValues[i], true);
            }
        }
    }
}