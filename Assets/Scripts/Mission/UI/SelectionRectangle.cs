using UnityEngine;
using UnityEngine.UI;

public class SelectionRectangle : MonoBehaviour
{
    // Reference to the RectTransform of the selection box
    private RectTransform selectionBoxRectTransform;

    // Reference to the Image component for visual representation
    private Image selectionBoxImage;

    void Start()
    {
        // Get the RectTransform and Image components
        selectionBoxRectTransform = GetComponent<RectTransform>();
        selectionBoxImage = GetComponent<Image>();

        // Ensure the script is attached to a UI object with RectTransform and Image components
        if (selectionBoxRectTransform == null || selectionBoxImage == null)
        {
            Debug.LogError("SelectionBox script requires RectTransform and Image components.");
            enabled = false; // Disable the script
        }
    }

    // Method to update the RectTransform size and position
    private void Update()
    {
        Vector2 startPos = PlayerInput.dragStart;
        Vector2 endPos = Input.mousePosition;//PlayerInput.dragEnd;

        // Calculate the size of the selection box
        Vector2 size = endPos - startPos;

        // Update the RectTransform size
        selectionBoxRectTransform.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

        // Calculate the center position of the selection box
        Vector2 center = startPos + 0.5f * size;

        // Update the RectTransform position
        selectionBoxRectTransform.position = center;
    }
}
