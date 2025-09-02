using UnityEngine;

using DG.Tweening;

public class CameraLook : MonoBehaviour
{
    [SerializeField]
    private float duration;

    public void LookAt(Transform target)
    {
        transform.DOLookAt(target.position, duration);
    }
}
