using UnityEngine;

public class TransformFollow : MonoBehaviour
{
    public Transform Follower;
    public Transform Target;
    public string TargetSubPath;
    public bool FollowInLateUpdate;

    private Transform RealTarget;
    private Matrix4x4 DiffMatrix;

    public void Rebind()
    {
        if (Target && !RealTarget)
        {
            if (TargetSubPath is null or "")
            {
                RealTarget = Target;
            }
            else
            {
                RealTarget = Target.Find(TargetSubPath);
            }
        }
        if (Follower && RealTarget)
        {
            DiffMatrix = RealTarget.worldToLocalMatrix * Follower.localToWorldMatrix;
        }
    }
    public void Follow()
    {
        if (Follower && RealTarget)
        {
            var mat = RealTarget.localToWorldMatrix * DiffMatrix;
            if (Follower.parent)
            {
                mat = Follower.parent.worldToLocalMatrix * mat;
            }
            Follower.localPosition = mat.GetPosition();
            Follower.localRotation = mat.rotation;
            Follower.localScale = mat.lossyScale;
        }
    }

    private void Awake()
    {
        if (Target && !Follower)
        {
            Follower = this.transform;
        }
        Rebind();
    }

    private void Update()
    {
        if (Target && !RealTarget)
        {
            Rebind();
        }
        if (!FollowInLateUpdate)
        {
            Follow();
        }
    }
    private void LateUpdate()
    {
        if (FollowInLateUpdate)
        {
            Follow();
        }
    }
}