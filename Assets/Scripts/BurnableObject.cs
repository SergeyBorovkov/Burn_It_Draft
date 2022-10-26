using UnityEngine;

public class BurnableObject : MonoBehaviour
{
    [SerializeField] private Collider _triggerCollider;
    [SerializeField] private ProjectorGroup _projectorGroup;
    [SerializeField] private BurnIconAnimator _burnerIcon;

    public void DisableTriggerCollider()
    {
        _triggerCollider.enabled = false;
    }

    public void ActivateProjectors()
    {
        _projectorGroup.Activate();
    }

    public void RemoveIcon()
    {
        _burnerIcon.AnimateDissappear();
    }
}