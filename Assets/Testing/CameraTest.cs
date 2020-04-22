using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{

    public Transform target;
    public float distance = 8.0f;
    public float xSpeed = 2.5f;
    public float ySpeed = 1.5f;
    public float yMinLimit = 5f;
    public float yMaxLimit = 90f;
    public float distanceMin = 3f;
    public float distanceMax = 20f;
    public float yOffset = 0.5f;
    public float rotationYAxis = 0.0f;
    public float rotationXAxis = 0.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    void LateUpdate()
    {
        if (target)
        {

            float velocityX = xSpeed * -Input.GetAxis("Horizontal") * Time.deltaTime * 100;
            float velocityY = ySpeed * -Input.GetAxis("Vertical") * Time.deltaTime * 100;
            
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
            distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position + new Vector3(0, yOffset, 0);

            transform.rotation = rotation;
            transform.position = position;

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
