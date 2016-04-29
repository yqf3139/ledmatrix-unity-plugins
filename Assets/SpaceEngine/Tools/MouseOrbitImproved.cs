using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
    public Transform target;
    public Transform followTarget;

    public float distance = 200f;
    public float xSpeed = 1.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = 5f;
    public float distanceMax = 500f;

    public float autoSpeed = 1f;
    public bool autoMove = false;
    public bool manualMove = true;

    private Rigidbody rigidbody;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }

    }

    void LateUpdate()
    {
        if (target)
        {
            if (!manualMove && !autoMove)
            {
                if (followTarget != null)
                {
                    Vector3 dir = 50 * Vector3.Normalize(followTarget.position - target.position);
                    dir.y = 200;
                    transform.position = followTarget.position + dir;
                    transform.LookAt(target);
                }
                return;
            }

            if (manualMove)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            if (autoMove)
            {
                x += Time.deltaTime * autoSpeed;
            }

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 20, distanceMin, distanceMax);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
        else
        {
            if (GameObject.Find("/Marker") != null)
            {
                target = GameObject.Find("/Marker").transform;
            }
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}