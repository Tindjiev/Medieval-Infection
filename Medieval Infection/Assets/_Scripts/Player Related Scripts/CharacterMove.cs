using UnityEngine;
using InputNM;
using MathNM;

public class CharacterMove : MonoBehaviour
{
    [SerializeField]
    private Transform _directionTr;

    [SerializeField]
    private Rigidbody _rb;

    public const float WALK_SPEED = 2.5f;
    private Vector3 _vectorMove;

    public Inputstruct MoveForwardInput = new Inputstruct(KeyCode.W);
    public Inputstruct MoveBackWardsInput = new Inputstruct(KeyCode.S);
    public Inputstruct MoveRightInput = new Inputstruct(KeyCode.D);
    public Inputstruct MoveLeftInput = new Inputstruct(KeyCode.A);


    protected void Awake()
    {
        if(!_directionTr) _directionTr = this.getvarsTR<CameraRotate>();
        if(!_rb) _rb = GetComponentInChildren<Rigidbody>();
    }

    protected void Update()
    {
        _vectorMove = Vector3.zero;
        if (MoveForwardInput.CheckInput())
        {
            _vectorMove = _directionTr.forward;
        }
        if (MoveBackWardsInput.CheckInput())
        {
            _vectorMove -= _directionTr.forward;
        }
        if (MoveRightInput.CheckInput())
        {
             _vectorMove += _directionTr.right;
        }
        if (MoveLeftInput.CheckInput())
        {
            _vectorMove -= _directionTr.right;
        }
        _vectorMove = WALK_SPEED * _vectorMove.normalized;
    }

    protected void FixedUpdate()
    {
        _rb.velocity = _vectorMove;
    }
}
