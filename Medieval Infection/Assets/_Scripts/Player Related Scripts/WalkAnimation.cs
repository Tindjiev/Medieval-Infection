using MathNM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WalkAnimation : MonoBehaviour
{

    private const float A = 0.05f;
    private const float T = MOVE_DURATION * 2f;
    private const float W = MathLib.TAU / T;

    private const float MOVE_DURATION = 2f / 3f;
    private const float STILL_DURATIOM = MOVE_DURATION * 0.15f;

    public UnityEvent RightFootStep { get; } = new UnityEvent();
    public UnityEvent LeftFootStep { get; } = new UnityEvent();


    [SerializeField]
    private Transform _face;


    private float _tField;
    private bool _moving = false;
    private bool _rightFoot = true;
    private bool _madeSound = false;

    private const float TOTAL_PERIOD = STILL_DURATIOM + MOVE_DURATION;

    private float _a = A;
    private float _t
    {
        get => _tField;
        set => _tField = value >= 0f ? value % TOTAL_PERIOD : -((-value) % TOTAL_PERIOD);
    }

    private Vector2 _lastFramePos;

    private float _height;

    private void Awake()
    {
        _t = 0f;
        _height = _face.localPosition.y;
        _lastFramePos = _face.position.toVector2();
    }

    void FixedUpdate()
    {
        Vector2 pos = _face.position.toVector2();
        bool notWalking = pos == _lastFramePos;

        if (!_moving)
        {
            if (notWalking)  return;
            _moving = true;
        }

        Vector3 localPos = _face.localPosition;
        if (notWalking && _t < MOVE_DURATION / 2f)
        {
            _a = localPos.y - _height;
            _t = MOVE_DURATION / 2f;
            _madeSound = true;
        }
        else
        {
            //localPos.y = _height + calcOffset(_x += (pos - _lastFramePos).magnitude);
            localPos.y = _height + calcOffset(_t);
            if (decideToMakeSound())
            {
                if (_rightFoot)
                {
                    RightFootStep.Invoke();
                }
                else
                {
                    LeftFootStep.Invoke();
                }
                _rightFoot = !_rightFoot;
            }
        }
        _face.localPosition = localPos;

        _lastFramePos = pos;
        _t += Time.fixedDeltaTime;
    }

    private float calcOffset(float x)
    {
        if (x < MOVE_DURATION)  return _a * Mathf.Sin(W * x).sq();

        _a = A;
        _moving = false;
        return 0f;
    }

    private bool decideToMakeSound()
    {
        if (_t > MOVE_DURATION / 2f)
        {
            if (!_madeSound)
            {
                _madeSound = true;
                return true;
            }
            return false;
        }
        _madeSound = false;
        return false;
    }
}
