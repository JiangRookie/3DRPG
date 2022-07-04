using UnityEngine;

public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;

    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        var dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;
    }
}