using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    private Vector3 lastCameraPos;
    public float parallaxFactor = 0.5f; // lower = slower movement (farther away)
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        lastCameraPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCameraPos;
        transform.position += new Vector3(delta.x * parallaxFactor, 0f, 0f);
        lastCameraPos = cam.position;
    }
}