using UnityEngine;

/// <summary>
/// 传送点
/// </summary>
public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,
        DifferentScene
    }

    [Header("传送门信息")]
    public string sceneName;
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag transitionDestinationTag;

    private bool m_bCanTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && m_bCanTrans)
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) m_bCanTrans = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) m_bCanTrans = false;
    }
}