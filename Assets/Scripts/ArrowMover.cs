using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ArrowMover : MonoBehaviour
{
    [SerializeField] private Image _arrowImagePrefab;

    private List<Image> _arrows = new List<Image>();
    private WaitForSeconds _waitInterval = new WaitForSeconds(0.75f);
    private Quaternion _defaultRotation = Quaternion.Euler(90, 0, 0);
    private float _moveDuration = 3;
    private float _arrowPoolCount = 4;
    private float _startPosZ = 32.2f;
    private float _endPosZ = 40.4f;
    private float _offsetX = 2f;    

    private void Start()
    {        
        InstantiateArrowPool();

        StartCoroutine(StartMove());
    }

    private IEnumerator StartMove()
    {
        foreach (var arrow in _arrows)
        {
            Tween(arrow);

            yield return _waitInterval;
        }            
    }

    private void Tween(Image arrow)
    {
        arrow.transform.DOMoveZ(_endPosZ, _moveDuration).SetLoops(-1).SetEase(Ease.Linear);
    }

    private void InstantiateArrowPool()
    {
        for (int i = 0; i < _arrowPoolCount; i++)
        {
            var newArrow = Instantiate(_arrowImagePrefab, _arrowImagePrefab.transform.position, _defaultRotation, transform);

            newArrow.rectTransform.anchoredPosition = _arrowImagePrefab.rectTransform.anchoredPosition;

            newArrow.transform.position = new Vector3(transform.position.x + _offsetX, transform.position.y, _startPosZ);
            
            _arrows.Add(newArrow);
        }
    }
}