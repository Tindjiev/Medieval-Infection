using UnityEngine;

public class CameraRotate : MonoBehaviour
{

    public float AngleBoundryUp = 85f;
    public float AngleBoundryDown = 55f;
    public float RotationSpeed = 2f;

    [SerializeField]
    private Camera _camera;

    public Rigidbody Rigidbody;
    public Transform ControlledTransform;

    public Camera Camera
    {
        get => _camera ? _camera : Camera.main;
        set => _camera = value;
    }

    private void Start()
    {
        if (!ControlledTransform) ControlledTransform = transform;
        if (!Rigidbody) Rigidbody = GetComponent<Rigidbody>();
    }



    protected void FixedUpdate()
    {
        Vector3 angles = (Rigidbody ? Rigidbody.rotation : ControlledTransform.rotation).eulerAngles;
        angles.x -= RotationSpeed * Input.GetAxis("Mouse Y");
        angles.y += RotationSpeed * Input.GetAxis("Mouse X");

        if (angles.x > AngleBoundryDown && angles.x < 180f)
        {
            angles.x = AngleBoundryDown;
        }
        else if (angles.x < 360f - AngleBoundryUp && angles.x > 180f)
        {
            angles.x = -AngleBoundryUp;
        }

        var tempRotation = Quaternion.Euler(angles);

        SetRotationToObjectControlled(tempRotation);

        //Camera.transform.rotation = tempRotation;
        //Camera.transform.position = transform.position;
    }



    private void SetRotationToObjectControlled(in Quaternion rotation)
    {
        if (Rigidbody)
        {
            Rigidbody.rotation = rotation;
        }
        else
        {
            ControlledTransform.rotation = rotation;
        }
    }

}
