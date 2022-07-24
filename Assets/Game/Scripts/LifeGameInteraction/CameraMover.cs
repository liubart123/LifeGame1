using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float moveSpeed, zoomSpeed;
    [Range(1f, 5f)]
    public float zoomInfluenceOnMoveSpeedPow;
    public Vector2 zoomLimit, moveLimit;
    Camera thisCamera;

    // Start is called before the first frame update
    void Start()
    {
        thisCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero;
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");
        direction *= moveSpeed * 
            Mathf.Pow(
                (thisCamera.orthographicSize - zoomLimit.x) /
                (zoomLimit.y - zoomLimit.x) + 1
                , zoomInfluenceOnMoveSpeedPow);
        transform.position += direction;

        Vector3 newPosition = new Vector3();
        newPosition.x = Mathf.Clamp(transform.position.x, -moveLimit.x, moveLimit.x);
        newPosition.y = Mathf.Clamp(transform.position.y, -moveLimit.y, moveLimit.y);
        newPosition.z = transform.position.z;
        transform.position = newPosition;

        thisCamera.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        thisCamera.orthographicSize = Mathf.Clamp(thisCamera.orthographicSize, zoomLimit.x, zoomLimit.y);
    }
}
