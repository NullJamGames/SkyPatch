using UnityEngine;

namespace NJG.Utilities
{
    /// <summary>
    ///     Provides utilities for generating and validating random points within specific
    ///     boundaries in a 2D space, useful for dynamic object placement in games.
    /// </summary>
    public static class PointService
    {
        public static Vector2 RandomPoint(float minX = 0, float minY = 0, float maxX = 1, float maxY = 1) =>
            new(Random.Range(minX, maxX), Random.Range(minY, maxY));

        public static Vector2 RandomPointInRadius(Vector2 center, float radius) =>
            center + Random.insideUnitCircle.normalized * radius;

        public static Vector2 RandomValidPoint(float minX, float minY, float maxX, float maxY, LayerMask mask)
        {
            bool isValid = false;
            Vector2? point = null;
            while (!isValid)
            {
                point = RandomPoint(minX, minY, maxX, maxY);
                Collider2D collider = Physics2D.OverlapPoint(point.Value, mask);
                isValid = collider == null;
            }

            return point.Value;
        }

        public static Vector2 RandomValidPointInRadius(Vector2 center, float radius, LayerMask mask)
        {
            bool isValid = false;
            Vector2? point = null;
            while (!isValid)
            {
                point = RandomPointInRadius(center, radius);
                Collider2D collider = Physics2D.OverlapPoint(point.Value, mask);
                isValid = collider == null;
            }

            return point.Value;
        }

        public static Vector2 DropPointToGround(Vector2 point, LayerMask ground, float distanceFromGround = 0.02F)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(point, Vector2.down, float.PositiveInfinity, ground);
            return hitInfo.point + Vector2.up * distanceFromGround;
        }
    }
}