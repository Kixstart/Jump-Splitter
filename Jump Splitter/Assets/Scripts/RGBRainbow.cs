using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RGBRainbow : MonoBehaviour
{
    [SerializeField] private Light2D light2D; // Serialized Light2D component
    [SerializeField] private SpriteRenderer spriteRenderer; // Serialized SpriteRenderer component
    public float speed = 7f; // Speed at which the color changes

    private float time;

    void Start()
    {
        // If not assigned in the Inspector, try to get the components
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (light2D != null && spriteRenderer != null)
        {
            // Update the time based on the speed
            time += Time.deltaTime * speed;

            // Create a rainbow effect using sin for a smooth transition between colors, ensuring they stay bright
            float red = Mathf.Abs(Mathf.Sin(time));  // Ensure values are between 0 and 1
            float green = Mathf.Abs(Mathf.Sin(time + Mathf.PI / 3)); // Phase shift for green
            float blue = Mathf.Abs(Mathf.Sin(time + Mathf.PI * 2 / 3)); // Phase shift for blue

            // Scale to full intensity for a bright effect (this ensures no dark colors)
            Color rainbowColor = new Color(red, green, blue);

            // Set the color for both the light and the sprite renderer
            light2D.color = rainbowColor;
            spriteRenderer.color = rainbowColor;
        }
    }
}