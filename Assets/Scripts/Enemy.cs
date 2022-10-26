using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _endPoint;
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _isMoving;
    [SerializeField] private Image _radarImage;
    [SerializeField] private CanvasGroup _alarmCanvasGroup;    
    [SerializeField] private GameObject _muzzleflash;    

    private Player _player;
    private Vector3 _startPosition;
    private Vector3 _startRotation;
    private Vector3 _firstRotation;
    private Vector3 _secondRotation;
    private Sequence _movingPatrolSequence;
    private Sequence _standPatrolSequence;
    private float _rotationDuration = 1;
    private float _walkDuration = 4;
    private float _idlePause = 2;
    private float _lookAtDuration = 0.05f;
    private float _muzzleFlashDelay = 0.15f;
    private float _damage = 0.1f;
    private float _animationTransitionDuration = 0.1f;
    private Vector3 _standPatrolRotation = new Vector3(0, 220, 0);
    private Vector3 _backRotation = new Vector3(0, 180, 0);
    private Vector3 _forwardRotation = new Vector3(0, 360, 0);
    private Color _redRadar = Color.red;
    private Color _defaultRadarColor;
    private int IdleHash = Animator.StringToHash("Idle");
    private int WalkHash = Animator.StringToHash("Walk");
    private int FireHash = Animator.StringToHash("Fire");
    private bool _isPlayerDetected;

    private void Start()
    {
        _defaultRadarColor = _radarImage.color;

        _startPosition = transform.position;

        _startRotation = transform.eulerAngles;
        
        if (_isMoving)
        {
            _firstRotation = _startRotation + _backRotation;

            _secondRotation = _endPoint.position.z < _startPosition.z ? _startRotation : _forwardRotation;

            AnimateMovingPatrol();
        }
        else
        {
            AnimateStandPatrol();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            _isPlayerDetected = true;

            PlayAlarmScenario(player);
        }       
    }

    private void PlayAlarmScenario(Player player)
    {
        _player = player;

        Animate(FireHash);        

        _alarmCanvasGroup.alpha = 1;        

        _radarImage.color = _redRadar;        

        LookAtPlayer(player);
    }

    private void AnimateMovingPatrol()
    {
        _movingPatrolSequence = DOTween.Sequence();

        _movingPatrolSequence.AppendCallback(() => Animate(IdleHash));
        _movingPatrolSequence.AppendInterval(_idlePause);
        _movingPatrolSequence.AppendCallback(() => Animate(WalkHash));
        _movingPatrolSequence.Append(transform.DOMoveZ(_endPoint.position.z, _walkDuration).SetEase(Ease.Linear));
        _movingPatrolSequence.Join(transform.DORotate(_firstRotation, _rotationDuration).SetEase(Ease.Linear));
        _movingPatrolSequence.AppendCallback(() => Animate(IdleHash));
        _movingPatrolSequence.AppendInterval(_idlePause);
        _movingPatrolSequence.AppendCallback(() => Animate(WalkHash));
        _movingPatrolSequence.Append(transform.DOMoveZ(_startPosition.z, _walkDuration).SetEase(Ease.Linear));
        _movingPatrolSequence.Join(transform.DORotate(_secondRotation, _rotationDuration).SetEase(Ease.Linear));
        _movingPatrolSequence.AppendCallback(() => Animate(WalkHash));
        _movingPatrolSequence.AppendCallback(() => AnimateMovingPatrol());        
    }

    private void AnimateStandPatrol()
    {
        if (_isPlayerDetected == false)
        {
            _standPatrolSequence = DOTween.Sequence();

            _standPatrolSequence.AppendCallback(() => Animate(IdleHash));
            _standPatrolSequence.Join(transform.DORotate(_standPatrolRotation, _rotationDuration).SetEase(Ease.Linear));
            _standPatrolSequence.AppendInterval(_idlePause);
            _standPatrolSequence.Append(transform.DORotate(_startRotation, _rotationDuration).SetEase(Ease.Linear));
            _standPatrolSequence.AppendInterval(_idlePause);
            _standPatrolSequence.AppendCallback(() => AnimateStandPatrol());
        }
        else
        {
            _movingPatrolSequence.Kill();
        }
    }

    private void Animate (int hashAnimation)
    {
        _animator.StopPlayback();
        _animator.CrossFade(hashAnimation, _animationTransitionDuration);        
    }

    private void Shoot()
    {
        if (_player != null)
        {
            _player.ReceiveDamage(_damage, transform.position, this);

            ShowMuzzleflash();
        }
    }

    private void ShowMuzzleflash()
    {
        _muzzleflash.SetActive(true);

        DOVirtual.DelayedCall(_muzzleFlashDelay, () => _muzzleflash.SetActive(false));
    }

    private void LookAtPlayer(Player player)
    {
        if (player.IsFinish == false)
        {
            Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);            

            transform.DOLookAt(targetPosition, _lookAtDuration).SetEase(Ease.InOutSine).OnComplete(() => LookAtPlayer(player));           

        }
        else
        {
            ExitAlarmScenario();
        }
    }

    private void ExitAlarmScenario()
    {
        Animate(IdleHash);

        _alarmCanvasGroup.alpha = 0;

        _radarImage.color = _defaultRadarColor;
    }
}