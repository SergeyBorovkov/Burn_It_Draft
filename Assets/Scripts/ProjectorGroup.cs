using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProjectorGroup : MonoBehaviour
{
    [SerializeField] private List<Projector> _projectors;
        
    private float _duration = 0.7f;
    private float _orthographicEndSize = 1.3f;

    public void Activate()
    {
        ChangeOrthographicSize();
    }

    private void ChangeOrthographicSize()
    {
        foreach (var projector in _projectors)
        {
            DOTween.To(() => projector.orthographicSize, x => projector.orthographicSize = x, _orthographicEndSize, _duration).SetEase(Ease.OutSine);
        }
    }
}