using UnityEngine;

/// <summary>
/// 传送终点
/// </summary>
public class TransitionDestination : MonoBehaviour
{
    /// <summary>
    /// 终点标签
    /// </summary>
    public enum DestinationTag
    {
        ENTER,
        A,
        B,
        C
    }

    public DestinationTag destinationTag;
}