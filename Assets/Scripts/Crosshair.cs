using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Color crosshairColor = Color.white;
    public float crosshairSize = 10f;
    [Range(0, 100)]
    public int thickness = 2;

    private Texture2D _texture;

    void Awake()
    {
        // Create a simple circular texture
        int size = 64;
        _texture = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist < radius && dist > radius - (thickness * (size / 20f)))
                {
                    _texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    _texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        _texture.Apply();
    }

    void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crosshairSize / 2);
        float yMin = (Screen.height / 2) - (crosshairSize / 2);
        
        GUI.color = crosshairColor;
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairSize, crosshairSize), _texture);
    }
}
