using NaughtyAttributes;
using UnityEngine;

public class FollowTarget : CameraMode
{
    
    [SerializeField] private bool lockToTarget = true;
    private bool ShowStaticAngles() => !lockToTarget;
    [ShowIf("ShowStaticAngles")][SerializeField] private float xAngle = 0f;
    [ShowIf("ShowStaticAngles")][SerializeField] private float yAngle = 0f;
    [ShowIf("ShowStaticAngles")][SerializeField] private float zAngle = 0f;

    [SerializeField] private float smoothDampDuration = 0.5f;
    private Vector3 velocity;

    public override void Activate()
    {
        base.Activate();
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    protected override void OnFixedUpdate()
    {
        if (Target && enabled)
        {
            Vector3 newPostion = Target.position;
            newPostion.x += xOffset;
            newPostion.y += yOffset;
            newPostion.z += zOffset;

            transform.position = Vector3.SmoothDamp(transform.position, newPostion, ref velocity, smoothDampDuration);

            if (lockToTarget)
            {
                transform.LookAt(Target);
            }
            else
            {
                transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle);
            }
        }
    }
}
