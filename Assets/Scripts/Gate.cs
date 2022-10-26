using UnityEngine;
using DG.Tweening;

public class Gate : MonoBehaviour
{
    [SerializeField] private GameObject _leftDoor;
    [SerializeField] private GameObject _rightDoor;
    [SerializeField] private float _openDistance;
    [SerializeField] private float _openDuration;

    private float _leftDoorStartPositionOnX;
    private float _rightDoorStartPositionOnX;

    private void Start()
    {
        _leftDoorStartPositionOnX = _leftDoor.transform.position.x;
        
        _rightDoorStartPositionOnX = _rightDoor.transform.position.x;

        Open();
    }

    public void Open()
    {
        _leftDoor.transform.DOMoveX(_leftDoorStartPositionOnX - _openDistance, _openDuration);

        _rightDoor.transform.DOMoveX(_rightDoorStartPositionOnX + _openDistance, _openDuration);
    }

    public void Close()
    {
        _leftDoor.transform.DOMoveX(_leftDoorStartPositionOnX, _openDuration);

        _rightDoor.transform.DOMoveX(_rightDoorStartPositionOnX, _openDuration);
    }
}