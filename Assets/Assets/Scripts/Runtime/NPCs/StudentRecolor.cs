using UnityEngine;

public class StudentRecolor : MonoBehaviour
{
    [Header("Targets (optional)")]
    [SerializeField] private SpriteRenderer hair;
    [SerializeField] private SpriteRenderer skin;
    [SerializeField] private SpriteRenderer clothes;
    [SerializeField] private SpriteRenderer hat;
    [SerializeField] private SpriteRenderer fallbackWhole; // used when parts are not separated

    [Header("Randomize (per instance)")]
    [SerializeField] private bool randomize = true;

    // Preset palettes (edit to your taste)
    private static readonly Color[] HairColors =
    {
        new Color32( 50,  32,  24, 255), // dark brown
        new Color32( 84,  60,  44, 255),
        new Color32( 25,  25,  25, 255), // black
        new Color32(160, 120,  72, 255)  // light brown
    };

    private static readonly Color[] SkinColors =
    {
        new Color32( 90,  58,  40, 255),
        new Color32(120,  80,  60, 255),
        new Color32(160, 110,  90, 255),
        new Color32(200, 160, 130, 255)
    };

    private static readonly Color[] ClothesColors =
    {
        new Color32(245, 135,  35, 255), // orange
        new Color32( 44, 150, 220, 255), // blue
        new Color32( 80, 180, 120, 255), // green
        new Color32(200,  80,  80, 255)  // red
    };

    private static readonly Color[] HatColors =
    {
        new Color32(255, 240,  30, 255), // yellow
        new Color32( 40, 200,  80, 255),
        new Color32( 30, 160, 220, 255),
        new Color32(120, 120, 120, 255)
    };

    private void Awake()
    {
        // Auto find a fallback if everything is empty
        if (hair == null && skin == null && clothes == null && hat == null && fallbackWhole == null)
        {
            fallbackWhole = GetComponent<SpriteRenderer>();
            if (fallbackWhole == null)
            {
                fallbackWhole = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (randomize) ApplyRandomColors();
    }

    public void ApplyRandomColors()
    {
        // If parts exist, tint parts; else tint whole
        if (hair != null || skin != null || clothes != null || hat != null)
        {
            if (hair != null) hair.color = Pick(HairColors);
            if (skin != null) skin.color = Pick(SkinColors);
            if (clothes != null) clothes.color = Pick(ClothesColors);
            if (hat != null) hat.color = Pick(HatColors);
        }
        else if (fallbackWhole != null)
        {
            // Whole-sprite tint: keep readable brightness
            Color c = Pick(ClothesColors);
            fallbackWhole.color = c;
        }
    }

    private static Color Pick(Color[] arr)
    {
        int idx = Random.Range(0, arr.Length);
        return arr[idx];
    }
}
