using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class FridgeTemperature : MonoBehaviour
{
    public float multiplier = 1.0f;
    public int period = 40;
    public int width;
    public int height;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private float[,] tempData;
    private int count = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create temp data
        tempData = new float[width, height];

        // Make a texture
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Apply blank pixels
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                texture.SetPixel(x, y, Color.black);

        /*for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x % 5 == 0)
                    tempData[x, y] = 1;
            }
        }*/

        texture.Apply();

        // Create sprite from texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 1);
        spriteRenderer.sprite = sprite;
    }

    void Update()
    {
        UpdateSinusoidalTemp();

       /* if (count > width * height)
            count = 0;

        int indexX = count % width;
        int indexY = count / height;*/

        /*if (indexX % 2 == 0)
            return;

        if (indexY % 2 == 1)
            return;

        float fullMult = Time.deltaTime * multiplier;

        Debug.Log($"Delta : {Time.deltaTime}, Value = {fullMult}");

        tempData[indexX, indexY] = Mathf.Clamp01(tempData[indexX, indexY] + fullMult);

        texture.SetPixel(indexX, indexY, new Color(0, 0, tempData[indexX, indexY]));*/

        //UpdatePixels();
       // SetConditions();

        //UpdateTemperature(indexX, indexY);

        
    }

    private void UpdatePixels()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Increase temperature value slowly (clamp between 0-1)
                tempData[x, y] = Mathf.Clamp01(tempData[x, y] + Time.deltaTime * 0.1f);

                // Map temperature to blue color (black to blue)
                texture.SetPixel(x, y, new Color(0, 0, tempData[x, y]));
            }
        }
    }

    private void SetConditions()
    {
        for (int x = 0; x < width; x++)
            tempData[x, 0] = 1;
    }

    private void UpdateTemperature(int width, int height)
    {
        if (width == 0) return;

        if (height == 0) return;

        if (width == this.width - 1) return;

        if (height == this.height - 1) return;

        float val = multiplier * (tempData[width - 1, height] + tempData[width + 1, height] + tempData[width, height - 1] + tempData[width, height + 1]);

        tempData[width, height] = Mathf.Clamp01(val / 4);

        texture.SetPixel(width, height, new Color(0, 0, tempData[width, height]));
    }

    private void UpdateSinusoidalTemp ()
    {
        multiplier = 1 + 0.01f * Mathf.Sin( 2 * 3.14f * ((float)count) / period);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetConditions();

                UpdateTemperature(x, y);
            }
        }

        count++;

        if (count > period)
            count = 0;

        //Apply the Texture Update
        texture.Apply();
    }

}
