using UnityEngine;
using TMPro;

/// <summary>
/// Tracks a given object with a UI element.
/// </summary>
public class ObjectiveMarker : MonoBehaviour
{
    /// <summary>
    /// How far away from edges of screen should this marker be?
    /// </summary>
    public int padding;
    /// <summary>
    /// How high above the objective should the marker sit?
    /// </summary>
    public int heightOffset;

    private TMP_Text label;
    private Transform objectiveTransform;
    private RectTransform rectTrans;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        label = GetComponentInChildren<TMP_Text>();
    }

    public void SetData(Transform objectToMark, string newLabel)
    {
        objectiveTransform = objectToMark;
        label.text = newLabel;
    }

    // Fixed update so it properly displays in case player may be moving with physics stuff.
    public void FixedUpdate()
    {
        if (objectiveTransform != null)
        {
            Vector3 objScreenPos = Camera.main.WorldToScreenPoint(objectiveTransform.position);
            objScreenPos.y += heightOffset;

            // make sure we're not off-screen
            objScreenPos = new Vector3(Mathf.Clamp(objScreenPos.x, padding, Screen.width - padding), Mathf.Clamp(objScreenPos.y, padding, (Screen.height - padding)), objScreenPos.z);

            rectTrans.position = objScreenPos;
        }
        else
        {
            // whenever the objectiveTransform is destroyed, destroy this marker.
            Destroy(gameObject);
        }
    }
}
