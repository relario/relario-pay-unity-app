using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PaymentButtonMenu : MonoBehaviour
{
    // MenuItem with the path "GameObject/UI/ButtonPrefab"
    [MenuItem("Relario/Payment Button")]
    static void AddButtonPrefabToCanvas()
    {
        // Load the button prefab from the Resources or Prefabs folder
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/RelarioPaymentButton");

        // Check if the button prefab is loaded
        if (buttonPrefab != null)
        {
            // Try to get the currently selected canvas
            GameObject rectTransform = Selection.activeGameObject;

            // If no canvas is selected, create a new one
            if (rectTransform == null || rectTransform.GetComponent<RectTransform>() == null)
            {
                rectTransform = CreateCanvas();
            }

            // Check if the canvas is still valid
            if (rectTransform != null && rectTransform.GetComponent<RectTransform>() != null)
            {
                // Create an instance of the prefab and set it as a child of the canvas
                GameObject buttonInstance = Instantiate(buttonPrefab, rectTransform.transform);
                buttonInstance.name = "RelarioPaymentButton"; // Optionally set the name
            }
            else
            {
                Debug.LogError("Failed to create or find a Canvas GameObject.");
            }
        }
        else
        {
            Debug.LogError("ButtonPrefab not found. Make sure it is in the Resources or Prefabs folder.");
        }
    }

    static GameObject CreateCanvas()
    {
        // Create a new canvas
        GameObject canvas = new GameObject("Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;

        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        // Create an event system if it doesn't exist
        if (GameObject.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // Set the canvas as the selection
        Selection.activeGameObject = canvas;

        return canvas;
    }
}