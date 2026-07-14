using UnityEngine;

/// <summary>
/// Simple component to identify statues for raycast detection.
/// Attach this to each statue GameObject.
/// </summary>
public class StatueInfo : MonoBehaviour
{
    [Header("Statue Identity")]
    [Tooltip("Unique ID matching the gateway instruction file (e.g., 'sekhmet', 'ramesses_ii')")]
    public string statueId = "sekhmet";
    
    [Tooltip("Display name shown in UI")]
    public string displayName = "سخمت";
    
    [Tooltip("English description shown in the Information panel")]
    [TextArea(3, 6)]
    public string description = "";
    
    [Tooltip("Gemini voice name (e.g., Charon, Puck, Kore, Fenrir, Aoede)")]
    public string voiceName = "";
    
    [Header("Visual Feedback")]
    [Tooltip("Optional: Highlight material when player looks at statue")]
    public Material highlightMaterial;
    
    private Material originalMaterial;
    private Renderer statueRenderer;
    
    private void Awake()
    {
        statueRenderer = GetComponent<Renderer>();
        if (statueRenderer != null)
        {
            originalMaterial = statueRenderer.material;
        }
    }
    
    public void SetHighlight(bool enabled)
    {
        if (statueRenderer != null && highlightMaterial != null)
        {
            statueRenderer.material = enabled ? highlightMaterial : originalMaterial;
        }
    }
}
