using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField]
    private Color defaultColor;

    [SerializeField]
    private Color hoverColor;

    private Material material;

    // Start is called before the first frame update
    private void Start()
    {
        // Get the material of the object
        material = GetComponent<MeshRenderer>().material;
        // Set the default color
        material.color = defaultColor;
    }

    private void OnMouseEnter() {
        // Set the hover color
        material.color = hoverColor;
    }

    private void OnMouseExit() {
        // Set the default color
        material.color = defaultColor;
    }

    // Destroy duplicated material when the highlight is destroyed
    private void OnDestroy() {
        Destroy(material);
    }
}
