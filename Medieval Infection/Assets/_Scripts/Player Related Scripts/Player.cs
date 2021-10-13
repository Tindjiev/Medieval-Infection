using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNM;

public class Player : MonoBehaviour
{
    [Zenject.Inject]
    public void Construct(DayNightCycle dayCycle)
    {
        dayCycle.EndayEvent.AddListener(Pause);
        dayCycle.AfterDayEvent.AddListener(() => { Position = _startingPosition; FacingAt = _startingFace; });
        dayCycle.NewDayEvent.AddListener(UnPause);
    }

    public Vector3 Position
    {
        get => _rb.position;
        set => transform.position = value;
        //set => _rb.MovePosition(value);
    }

    private Vector3 _startingPosition;
    private Vector3 _startingFace;

    private Transform _face;
    private Rigidbody _rb;
    public Vector3 FacingAt
    {
        get => _face.forward;
        set => _face.forward = value;
    }


    private void Awake()
    {
        _face = GetComponentInChildren<CameraRotate>().transform;
        _rb = GetComponent<Rigidbody>();

        if (_rb == null) _rb = GetComponentInChildren<Rigidbody>();
    }

    private void Start()
    {
        _startingPosition = Position;
        _startingFace = FacingAt;
    }

    private void OnEnable()
    {
        loadScene_setPlayer();
    }


    public void Pause()
    {
        GetComponentInChildren<CharacterMove>().enabled = false;
        GetComponentInChildren<CameraRotate>().enabled = false;
        GetComponentInChildren<PlayerInteractor>().enabled = false;
    }

    public void UnPause()
    {
        GetComponentInChildren<CharacterMove>().enabled = true;
        GetComponentInChildren<CameraRotate>().enabled = true;
        GetComponentInChildren<PlayerInteractor>().enabled = true;
    }


    private static Transform _staticDoor;

    public static void StoreParametersExitingHouse(Door door)
    {
        _staticDoor = door.transform;
    }

    private void loadScene_setPlayer()
    {
        if (_staticDoor == null) return;

        Position = _staticDoor.position + 2f * (FacingAt = _staticDoor.forward);
        GetComponent<AudioSource>().Play();

        _staticDoor = null;

    }

}
