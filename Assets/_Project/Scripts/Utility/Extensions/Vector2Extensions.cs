using System;
using UnityEngine;

namespace NJG.Utilities.Extensions
{
    /// <summary>
    ///     Enhances Unity's Vector2 with additional vector operations, facilitating complex vector manipulations such as
    ///     projection, orthogonal calculation, and component-wise adjustments, useful for detailed movement and
    ///     interaction mechanics in game development.
    /// </summary>
    public static class Vector2Extensions
    {
        public static Vector2 ProjectOnto(this Vector2 a, Vector2 b) => Vector2.Dot(a, b) / b.sqrMagnitude * b;

        public static Vector2 Orthogonal(this Vector2 self) => new(-self.y, self.x);

        public static Vector2 Clone(this Vector2 self) => new(self.x, self.y);

        /// <summary>
        ///     Adds to any x y values of a Vector2
        /// </summary>
        public static Vector2 Add(this Vector2 vector2, float x = 0, float y = 0) => new(vector2.x + x, vector2.y + y);

        public static Vector2 Multiply(this Vector2 self, Vector2 other) => new(self.x * other.x, self.y * other.y);

        /// <summary>
        ///     Sets any x y values of a Vector2
        /// </summary>
        public static Vector2 With(this Vector2 vector2, float? x = null, float? y = null) =>
            new(x ?? vector2.x, y ?? vector2.y);

        public static Vector2 WithX(this Vector2 self, float x) => new(x, self.y);

        public static Vector2 WithY(this Vector2 self, float y) => new(self.x, y);

        public static Vector4 Concat(this Vector2 self, Vector2 other) => new(self.x, self.y, other.x, other.y);

        public static float Dot(this Vector2 self, Vector2 other) => self.x * other.x + self.y * other.y;

        public static float AsScalarOf(this Vector2 self, Vector2 other)
        {
            float scalar;
            if (other.x != 0)
                scalar = self.x / other.x;
            else if (other.y != 0)
                scalar = self.y / other.y;
            else
                // other is {0,0}, so…
                return 0;

            Vector2 scaledOther = other * scalar;

            float tolerance = 1e-5F;
            if (scaledOther.x - self.x < tolerance && scaledOther.y - self.y < tolerance)
                return scalar;

            throw new InvalidOperationException("Vector parameter is not a scalar of target Vector");
        }

        /// <summary>
        ///     Returns a Boolean indicating whether the current Vector2 is in a given range from another Vector2
        /// </summary>
        /// <param name="current">The current Vector2 position</param>
        /// <param name="target">The Vector2 position to compare against</param>
        /// <param name="range">The range value to compare against</param>
        /// <returns>True if the current Vector2 is in the given range from the target Vector2, false otherwise</returns>
        public static bool InRangeOf(this Vector2 current, Vector2 target, float range) =>
            (current - target).sqrMagnitude <= range * range;
    }
}