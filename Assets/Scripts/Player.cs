using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private VariableJoystick _joystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<Transform> _jumpOverPoints;
    [SerializeField] private CanvasGroup _joystickCanvasGroup;
    [SerializeField] private CanvasGroup _healthbarCanvasGroup;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _leftCanister;
    [SerializeField] private GameObject _rightCanister;
    [SerializeField] private Slider _healthbarSlider;

    public bool IsFinish => _isFinish;

    private List<BurnableObject> _burners = new List<BurnableObject>();    
    private Sequence _jumpOver;
    private WaitForSeconds _jumpOverInterval;
    private Enemy _activeEnemy;
    private float _currentHealth = 1;
    private float _jumpOverDuration = 0.6f;
    private float _showJoystickPause = 1.7f;
    private float _rotationAngle;
    private float _rotationSpeed = 8;
    private float _moveSpeed = 4;
    private float _animationTransitionDuration = 0.1f;
    private float _burnerIconDissappearDelay = 0.5f;
    private float _projectorActivationDelay = 0.4f;
    private float _splashingDeactivationDelay = 1f;
    private float _throwMatchAnimationDelay = 0.7f;
    private float _finishWalkingLengthOnZ = 5f;
    private float _finishWalkingDuration = 5f;
    private float _healthbarCanvasDeactivationDelay = 1f;
    private int RunHash = Animator.StringToHash("Run");
    private int IdleHash = Animator.StringToHash("Idle");
    private int JumpOverHash = Animator.StringToHash("JumpOver");
    private int SplashHash = Animator.StringToHash("Splash");
    private int WalkHash = Animator.StringToHash("Walk");
    private int ThrowMatchHash = Animator.StringToHash("Throw");
    private int TakeShotTriggerHash = Animator.StringToHash("TakeShot");
    private int MirrorTakeShotBoolHash = Animator.StringToHash("MirrorTakeShot");
    private bool _isRunning;
    private bool _isSplashing;
    private bool _isFinish;

    private void Update()
    {
        if (_isFinish == false)
        {
            Rotate();

            Move();
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<BurnableObject>(out BurnableObject burner) && _rotationAngle == 0)
        {
            _isSplashing = true;

            Animate(SplashHash);
            
            burner.DisableTriggerCollider();

            DOVirtual.DelayedCall(_projectorActivationDelay, burner.ActivateProjectors);

            DOVirtual.DelayedCall(_burnerIconDissappearDelay, burner.RemoveIcon);

            Invoke(nameof(SetSplashFalse), _splashingDeactivationDelay);
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Finish>())
        {
            _isFinish = true;
            
            _joystickCanvasGroup.alpha = 0;

            Animate(WalkHash);

            DOVirtual.DelayedCall(_throwMatchAnimationDelay, () => Animate(ThrowMatchHash));

            _rightCanister.SetActive(false);

            _leftCanister.SetActive(true);

            transform.eulerAngles = Vector3.zero;

            transform.DOMoveZ(transform.position.z + _finishWalkingLengthOnZ, _finishWalkingDuration).SetEase(Ease.Linear);            

            DOVirtual.DelayedCall(_healthbarCanvasDeactivationDelay, () => _healthbarCanvasGroup.alpha = 0);            
        }
    }

    public void ReceiveDamage(float hitpoints, Vector3 enemyPosition, Enemy enemy)
    {
        _activeEnemy = enemy;

        _healthbarCanvasGroup.alpha = 1;

        float newHealth = _currentHealth - hitpoints;

        DOTween.To(() => _healthbarSlider.value, x => _healthbarSlider.value = x, newHealth, hitpoints).SetEase(Ease.OutSine);
            
        _currentHealth = newHealth;
        
        Vector3 localEnemyPosition = transform.InverseTransformPoint(enemyPosition);        

        if (localEnemyPosition.x > 0)
            _animator.SetBool(MirrorTakeShotBoolHash, false);        
        else
            _animator.SetBool(MirrorTakeShotBoolHash, true);        

        _animator.SetTrigger(TakeShotTriggerHash);
    }

    public void StartJumpOver()
    {
        StartCoroutine(JumpOver());

        Invoke(nameof(ShowJoystick), _showJoystickPause);
    }

    private void ShowJoystick()
    {
        _joystickCanvasGroup.alpha = 1;
    }

    private IEnumerator JumpOver()
    {
        float interval = _jumpOverDuration / _jumpOverPoints.Count;

        _jumpOverInterval = new WaitForSeconds(interval);

        _jumpOver = DOTween.Sequence();

        Animate(JumpOverHash);

        foreach (var point in _jumpOverPoints)
        {
            _jumpOver.Append(transform.DOMove(point.position, _jumpOverDuration)).SetEase(Ease.Linear);

            yield return _jumpOverInterval;
        }        
    }

    private void Rotate()
    {
        _rotationAngle = Mathf.Atan2(_joystick.Horizontal, _joystick.Vertical) * Mathf.Rad2Deg;

        if (_rotationAngle != 0)        
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, _rotationAngle, 0)), Time.deltaTime * _rotationSpeed);        
    }
    
    private void Move()
    {
        if (_rotationAngle != 0)
        {
            if (_isRunning == false)
            {
                Animate(RunHash);

                _isRunning = true;
            }

            transform.position = new Vector3(transform.position.x + transform.forward.x * Time.deltaTime * _moveSpeed,
                0, transform.position.z + transform.forward.z * Time.deltaTime * _moveSpeed);
        }
        else
        {
            if (_isRunning == true && _isSplashing == false)
            {
                _isRunning = false;

                Animate(IdleHash);
            }
        }
    }

    private void Animate(int hashAnimation)
    {
        _animator.StopPlayback();
        _animator.CrossFade(hashAnimation, _animationTransitionDuration);        
    }

    private void SetSplashFalse()
    {
        _isSplashing = false;
    }
}