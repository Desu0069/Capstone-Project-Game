using UnityEngine;

public class PlayerPusher : MonoBehaviour
{
    public float pushPower = 2f;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var pushable = hit.collider.GetComponent<PushableObject>();

        if (pushable == null)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        pushable.Push(pushDir * pushPower);
    }
}
