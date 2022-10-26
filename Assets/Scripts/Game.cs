using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _playerMover;
    [SerializeField] private CinemachineVirtualCamera _startCamera;
    [SerializeField] private CinemachineVirtualCamera _playCamera;
    [SerializeField] private float _startCameraEndPointOnZ;
    [SerializeField] private float _startCameraMoveDuration;

    private float _startCameraDelay = 0.5f;

    private void Start()
    {
        _playCamera.Priority = 0;

        Invoke(nameof(ActivateStartCamera), _startCameraDelay);
    }

    private void ActivateStartCamera()
    {
        _startCamera.Priority++;

        _startCamera.transform.DOMoveZ(_startCameraEndPointOnZ, _startCameraMoveDuration).OnComplete(ActivatePlayCamera);
    }

    private void ActivatePlayCamera()
    {
        _startCamera.Priority = 0;

        _playCamera.Priority = 1;

        _playerMover.StartJumpOver();
    }
}