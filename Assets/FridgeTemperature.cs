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
        //UpdateSinusoidalTemp();
        UpdateRegularTemp();
    }

    /// <summary>
    /// First method for Applying Values to Texture 2D
    /// </summary>
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

    /// <summary>
    /// Straight Line Condition
    /// </summary>
    private void SetLineCondition()
    {
        for (int x = 0; x < width; x++)
            tempData[x, height / 2] = 1;
    }

    /// <summary>
    /// Small Sqquare Condition
    /// </summary>
    private void SetSquareCondition()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                tempData[35 + i, 70 + j] = 1;
            }
        }
    }

    private void SetFloatingSquareCondition()
    {
        int sinX = (int)(25 * Mathf.Sin(2 * 3.14f * ((float)count) / period));
        int sinY = (int)(25 * Mathf.Sin(2 * 3.14f * 3 * ((float)count) / period));

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                tempData[50 + i + sinX, 50 + j + sinY] = 1;
            }
        }
    }

    private void UpdateTemperature(int width, int height)
    {
        float val = 0;

        //Corners need to be implemented
        if (width == 0 && height == 0)
            return;

        if (width == 0 && height == this.height - 1)
            return;

        if (width == this.width - 1 && height == 0)
            return;

        if (width == this.width - 1 && height == this.height - 1)
            return;

        if (width == 0)
            val = 2 * tempData[width + 1, height] + tempData[width, height - 1] + tempData[width, height + 1];

        else if (width == this.width - 1)
            val = 2 * tempData[width - 1, height] + tempData[width, height - 1] + tempData[width, height + 1];

        else if (height == 0)
            val = tempData[width - 1, height] + tempData[width + 1, height] + 2 * tempData[width, height + 1];

        else if (height == this.height - 1)
            val = tempData[width - 1, height] + tempData[width + 1, height] + 2 * tempData[width, height - 1];

        else
            val = (tempData[width - 1, height] + tempData[width + 1, height] + tempData[width, height - 1] + tempData[width, height + 1]);

        val *= multiplier;

        tempData[width, height] = Mathf.Clamp01(val / 4);

        texture.SetPixel(width, height, new Color(0, 0, tempData[width, height]));
    }

    private void UpdateRegularTemp ()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //SetSquareCondition();

                SetFloatingSquareCondition();

                UpdateTemperature(x, y);
            }
        }

        count++;

        if (count > period)
            count = 0;

        //Apply the Texture Update
        texture.Apply();
    }

    private void UpdateSinusoidalTemp()
    {
        multiplier = 1 + 0.01f * Mathf.Sin(2 * 3.14f * ((float)count) / period);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //SetSquareCondition();

                SetFloatingSquareCondition();

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
