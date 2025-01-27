using UnityEngine;
using UnityEngine.UI;

public class SplitScreenCamera : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 8, -10);
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Color borderColor = Color.black;
    [SerializeField] private float borderThickness = 4f;

    private Camera player1Camera;
    private Camera player2Camera;
    private GameObject uiCanvas;

    // 1) Add a reference to your bubble Transform
    private Transform bubble;

    private void Start()
    {
        SetupCameras();
        CreateBorders();

        // 2) Find the Bubble by tag (make sure you have exactly one object tagged "Bubble"!)
        GameObject bubbleObject = GameObject.FindGameObjectWithTag("Bubble");
        if (bubbleObject != null)
        {
            bubble = bubbleObject.transform;
        }
        else
        {
            Debug.LogWarning("No object found with tag 'Bubble'. Make sure you've assigned the tag in the Inspector!");
        }
    }

    private void LateUpdate()
    {
        if (player1 != null && player2 != null)
        {
            UpdateCameraPositions();
        }
    }

    private void SetupCameras()
    {
        if (player1Camera == null)
        {
            GameObject cam1 = new GameObject("Player1Camera");
            player1Camera = cam1.AddComponent<Camera>();
            cam1.transform.parent = transform;
        }

        if (player2Camera == null)
        {
            GameObject cam2 = new GameObject("Player2Camera");
            player2Camera = cam2.AddComponent<Camera>();
            cam2.transform.parent = transform;
        }

        player1Camera.rect = new Rect(0, 0, 0.5f, 1);
        player2Camera.rect = new Rect(0.5f, 0, 0.5f, 1);
    }

    private void CreateBorders()
    {
        // ... same as before ...
        uiCanvas = new GameObject("BorderCanvas");
        Canvas canvas = uiCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.AddComponent<CanvasScaler>();

        GameObject centerLine = new GameObject("CenterLine");
        centerLine.transform.SetParent(uiCanvas.transform, false);
        UnityEngine.UI.Image centerImage = centerLine.AddComponent<UnityEngine.UI.Image>();
        centerImage.color = borderColor;
        RectTransform centerRect = centerImage.GetComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.5f, 0);
        centerRect.anchorMax = new Vector2(0.5f, 1);
        centerRect.sizeDelta = new Vector2(borderThickness, 0);
        centerRect.anchoredPosition = Vector2.zero;

        // Create horizontal and vertical borders
        CreateBorderLine("TopBorder", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, borderThickness));
        CreateBorderLine("BottomBorder", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, borderThickness));
        CreateBorderLine("LeftBorder", new Vector2(0, 0), new Vector2(0, 1), new Vector2(borderThickness, 0));
        CreateBorderLine("RightBorder", new Vector2(1, 0), new Vector2(1, 1), new Vector2(borderThickness, 0));
    }

    private void CreateBorderLine(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta)
    {
        // ... same as before ...
        GameObject border = new GameObject(name);
        border.transform.SetParent(uiCanvas.transform, false);
        Image borderImage = border.AddComponent<Image>();
        borderImage.color = borderColor;
        RectTransform rectTransform = borderImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = sizeDelta;
    }

    private void UpdateCameraPositions()
    {
        // Follow player 1
        Vector3 desiredPosition1 = player1.position + cameraOffset;
        Vector3 smoothedPosition1 = Vector3.Lerp(player1Camera.transform.position, desiredPosition1, smoothSpeed);
        player1Camera.transform.position = smoothedPosition1;

        // Follow player 2
        Vector3 desiredPosition2 = player2.position + cameraOffset;
        Vector3 smoothedPosition2 = Vector3.Lerp(player2Camera.transform.position, desiredPosition2, smoothSpeed);
        player2Camera.transform.position = smoothedPosition2;

        // 3) Now make both cameras face the bubble, if we found it
        if (bubble != null)
        {
            player1Camera.transform.LookAt(bubble);
            player2Camera.transform.LookAt(bubble);
        }
        else
        {
            // Fallback if no bubble found
            player1Camera.transform.LookAt(player1.position);
            player2Camera.transform.LookAt(player2.position);
        }
    }
}
