using UnityEngine;

public class CameraController : MonoBehaviour
{
    //The targetObject
    public Transform targetPlayer;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - targetPlayer.position;
    }

    void Update()
    {
        Vector3 newCameraPosition = new Vector3(transform.position.x, transform.position.y, offset.z + targetPlayer.position.z);
        transform.position = newCameraPosition;
    }
}

