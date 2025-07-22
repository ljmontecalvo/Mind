using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.2f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = offset.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
