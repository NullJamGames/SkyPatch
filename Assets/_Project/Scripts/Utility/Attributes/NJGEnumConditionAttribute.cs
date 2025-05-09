using System;
using System.Collections;
using UnityEngine;

namespace NJG.Utilities
{
    /// <summary>
    ///     An attribute to conditionally hide fields based on the current selection in an enum.
    ///     Usage :  [NJGEnumCondition("resourceType", false, (int)ResourceType.Food, (int)ResourceType.Water)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class NJGEnumConditionAttribute : PropertyAttribute
    {
        private readonly BitArray bitArray = new(32);
        public string ConditionEnum = "";
        public bool Hidden;
        public bool Inverse;

        public NJGEnumConditionAttribute(string conditionEnum, bool inverse, params int[] enumValues)
        {
            ConditionEnum = conditionEnum;
            Hidden = true;
            Inverse = inverse;

            for (int i = 0; i < enumValues.Length; i++)
                bitArray.Set(enumValues[i], true);
        }

        public bool ContainsBitFlag(int enumValue) => bitArray.Get(enumValue) ^ Inverse; // Apply inverse logic
    }
}