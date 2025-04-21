using UnityEngine;

public class ballIndicatormngr : MonoBehaviour
{
    public GameObject ball;
    public GameObject Indicator;
    public float out_of_screen_y = 1f;
    public float y_fixed_offset = 0.9f;

    private Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        setState();
        track();
    }
    void track(){
        if (ball != null && Indicator != null)
        {
            Vector3 objectScreenPos = mainCamera.WorldToScreenPoint(ball.transform.position);

            // Set the X position of the indicator
            float screenX = objectScreenPos.x;

            // Ensure that the indicator stays within the screen bounds (horizontal edge clamping)
            screenX = Mathf.Clamp(screenX, 0, Screen.width);

            // Set the fixed Y position for the indicator in screen space
            float screenY = Screen.height * 0.95f; // Example: 95% of the screen height, you can adjust as needed


            // Convert the screen position back to world space
            Vector3 indicatorWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenX, screenY, objectScreenPos.z));

            // Update the indicator position (assuming it's a UI element, so use RectTransform)
            Indicator.transform.position = indicatorWorldPos;
        }
    }

    void setState(){
        Vector3 objectScreenPos = mainCamera.WorldToScreenPoint(ball.transform.position);
        if(objectScreenPos.y > (Screen.height * 1.1f)){
            Indicator.SetActive(true);  
        }else{
            Indicator.SetActive(false);
        }
    }
}


    