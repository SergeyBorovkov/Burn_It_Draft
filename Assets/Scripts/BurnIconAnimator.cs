using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BurnIconAnimator : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TrailRenderer _trailPrefab;
    [SerializeField] private Camera _mainCamera;
        
    private TrailRenderer _trail;
    private Tween _idleMove;
    private Tween _idleRotate;
    private Vector3 _idleRotationAngles = new Vector3(0, 0, 15);
    private float _rotateIdleDuration = 0.8f;
    private float _moveIdleDuration = 0.4f;
    private float _moveIdleDistance = 0.2f;
    private float _moveToCornerDuration = 0.3f;
    private float _beyondScreenOffset = 0.1f;

    private void Start()
    {
        _icon.transform.localEulerAngles =- _idleRotationAngles;

        InstantiateTrail();

        AnimateIdle();        
    }

    public void AnimateDissappear()
    {
        _idleMove.Kill();
        _idleRotate.Kill();

        MoveToUppperLeftCorner();

        _trail.emitting = true;
    }

    private void InstantiateTrail()
    {
        _trail = Instantiate(_trailPrefab, _icon.transform.position, Quaternion.identity, _icon.transform);

        _trail.emitting = false;
    }

    private void AnimateIdle()
    {
        _idleMove = _icon.transform.DOMoveY(_icon.transform.position.y + _moveIdleDistance, _moveIdleDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);

        _idleRotate = _icon.transform.DOLocalRotate(_idleRotationAngles, _rotateIdleDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    private void MoveToUppperLeftCorner()
    {
        float iconCameraDistance = _mainCamera.WorldToScreenPoint(_icon.transform.position).z;
        
        var upperLeftCornerWorldPoint = _mainCamera.ViewportToWorldPoint(new Vector3(0 - _beyondScreenOffset * _mainCamera.pixelHeight / _mainCamera.pixelWidth, 
            1 + _beyondScreenOffset, iconCameraDistance));     

        _icon.transform.DOMove(upperLeftCornerWorldPoint, _moveToCornerDuration).OnComplete(Deactivate);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}