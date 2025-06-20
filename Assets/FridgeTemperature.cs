using UnityEngine;
using UnityEngine.InputSystem;

public class FridgeTemperature : MonoBehaviour
{
    public Camera camera;
    public float multiplier = 1.0f;
    public int period = 80;
    public int width;
    public int height;
    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    public float[,] tempData;
    private int count = 0;
    public float AvgTemp;
    public InputAction inputAction;

    public int ChildCount;
    public Transform[] Children;

    private void OnEnable()
    {
        inputAction.Enable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    public enum HeatDiffusionCase
    {
        Middle,
        Top,
        Bottom,
        Right,
        Left,
        BottomLeft,
        BottomRight,
        TopRight,
        TopLeft
    }

    public HeatDiffusionCase GetDiffusionCase(int width, int height)
    {
        int leftSide = this.width - 1;
        int topSide = this.height - 1;

        if (width == 0 && height == 0)
            return HeatDiffusionCase.BottomLeft;
        else if (width == leftSide && height == 0)
            return HeatDiffusionCase.BottomRight;
        else if (width == 0 && height == topSide)
            return HeatDiffusionCase.TopLeft;
        else if (width == leftSide && height == topSide)
            return HeatDiffusionCase.TopRight;
        else if (width == 0)
            return HeatDiffusionCase.Left;
        else if (width == leftSide)
            return HeatDiffusionCase.Right;
        else if (height == topSide)
            return HeatDiffusionCase.Top;
        else if (height == 0)
            return HeatDiffusionCase.Bottom;
        else
            return HeatDiffusionCase.Middle;
    }

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

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                tempData[x, y] = 0.5f;

                if (x < 3 || x > this.width - 3)
                    continue;

                if (y < 3 || y > this.height - 3)
                    continue;

                if (y % 10 == 0)
                {
                    float randTemp = 0.5f + Random.Range(-0.3f, 0.3f);

                    for (int i = -2; i < 2; i++)
                        for (int j = -2; j < 2; j++)
                            tempData[x + i, y + j] = randTemp;

                }
            }
                

        texture.Apply();

        // Create sprite from texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 1);
        spriteRenderer.sprite = sprite;

        //Fit Camera to height of the fridge (+ 5% buffer)
        float orthoSizeHeight = spriteRenderer.bounds.size.y / 2f;
        camera.orthographicSize = orthoSizeHeight * 1.05f;

        UpdateChildren();
    }

    public void UpdateChildren()
    {
        Children = new Transform[ChildCount];

        for (int i = 0; i < ChildCount; i++)
        {
            Children[i] = transform.GetChild(i);

            if (Children[i].GetComponent<TemperatureSprite>())
                Children[i].GetComponent<TemperatureSprite>().SetParentSize(this.spriteRenderer.bounds.size);
            else if (Children[i].GetComponent<Vent>())
                Children[i].GetComponent<Vent>().SetParentSize(this.spriteRenderer.bounds.size);
        }
    }

    private void FixedUpdate()
    {
        UpdateRegularTemp();

        float sum = 0;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                sum += tempData[x, y];

        AvgTemp = sum / tempData.Length;
    }

    void Update()
    {
        //UpdateSinusoidalTemp();
        

        //Debug.Log($"Average Temp of Fridge : {AvgTemp}");

        //if (inputAction.ReadValue<float>() == 1f)
        //    this.transform.GetComponent<SpriteRenderer>().enabled = true;
        //else
        //    this.transform.GetComponent<SpriteRenderer>().enabled = false;
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

    public float GetTempMultiplier (TempSprite sprite)
    {
        if (sprite.Temperature < 0.5f &&  AvgTemp > 0.6f)
            return 0.8f;

        if (sprite.Temperature > 0.5f && AvgTemp < 0.4f)
            return 1.2f;

        return 1f;
    }

    private void SetSpriteTemp()
    {
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        /*foreach (Transform child in Children)
        {
            TempSprite sprite = child.GetComponent<TempSprite>();

            if (sprite == null)
                continue;

            if (!sprite.IsActive)
                continue;

            float multiplier = GetTempMultiplier(sprite);

            Vector3 childPos = child.position;

            int x = (int)childPos.x;
            int y = (int)childPos.y;
            int size = sprite.TempSize;

            float temp = sprite.Temperature * multiplier;

            for (int w = -size; w < size + 1; w++)
            {
                for (int h = -size; h < size + 1; h++)
                {
                    tempData[x + w + halfWidth, y + h + halfHeight] = temp;
                }
            }

        }*/





        foreach (Transform child in Children)
        {
            TemperatureSprite sprite = child.GetComponent<TemperatureSprite>();

            if (sprite == null)
                continue;

            float multiplier = GetTempMultiplier(sprite);

            Vector3 childPos = child.position;

            int x = (int)childPos.x;
            int y = (int)childPos.y;

            float temp = sprite.Temperature * multiplier;

            for (int w = -1; w < 2; w++)
            {
                for (int h = -1; h < 2; h++)
                {
                    tempData[x + w + halfWidth, y + h + halfHeight] = temp;
                }
            }
        }

        foreach (Transform child in Children)
        {
            Vent vent = child.GetComponent<Vent>();

            if (vent == null)
                continue;

            if (!vent.IsActive)
                continue;

            Vector3 childPos = child.position;

            int x = (int)childPos.x;
            int y = (int)childPos.y;

            int size = vent.TempSize;

            for (int w = -size; w < size + 1; w++)
            {
                for (int h = -size; h < size +1 ; h++)
                {
                    tempData[x + w + halfWidth, y + h + halfHeight] = vent.Temperature;
                }
            }







            /*float multiplier = GetTempMultiplier(vent);

            Vector3 childPos = child.position;

            int x = (int)childPos.x;
            int y = (int)childPos.y;

            float temp = vent.Temperature * multiplier;

            for (int w = -1; w < 1; w++)
            {
                for (int h = -1; h < 1; h++)
                {
                    tempData[x + w + halfWidth, y + h + halfHeight] = temp;
                }
            }*/
        }
    }

    private void SetFloatingSquareCondition(int startX, int startY, int sizeX, int sizeY, float tempValue)
    {
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        int sinX = (int)(halfWidth * 0.25 * Mathf.Sin(2 * 3.14f * ((float)count) / period));
        int sinY = (int)(halfHeight * 0.25 * Mathf.Sin(2 * 3.14f * 4 * ((float)count) / period));

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                tempData[startX + i + sinX, startY + j + sinY] = tempValue;
            }
        }
    }

    private void SetFloatingSquareCondition()
    {
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        int sinX = (int)(halfWidth * 0.25 * Mathf.Sin(2 * 3.14f * ((float)count) / period));
        int sinY = (int)(halfHeight * 0.25 * Mathf.Sin(2 * 3.14f * 4 * ((float)count) / period));

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                tempData[halfWidth + i + sinX, halfHeight + j + sinY] = 1;
            }
        }
    }

    private void UpdateTemperature(int width, int height)
    {
        float val = 0;

        //Corners need to be implemented

        //Bottom Left
        if (width == 0 && height == 0)
            val = 2 * tempData[width + 1, height] + 2 * tempData[width, height + 1];
        //Top Left
        else if (width == 0 && height == this.height - 1)
            val = 2 * tempData[width + 1, height] + 2 * tempData[width, height - 1];
        //Bottom right
        else if (width == this.width - 1 && height == 0)
            val = 2 * tempData[width - 1, height] + 2 * tempData[width, height + 1];
        //Top Right
        else if (width == this.width - 1 && height == this.height - 1)
            val = 2 * tempData[width - 1, height] + 2 * tempData[width, height - 1];
        else if (width == 0)
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


        texture.SetPixel(width, height, GetColor(tempData[width, height]));

       /* if (tempData[width, height] > 0.5f)
            texture.SetPixel(width, height, new Color((tempData[width, height] * 2) - 0.5f, 0, 0, 0.3f));
        else
            texture.SetPixel(width, height, new Color(0, 0, 1 - (tempData[width, height] * 2), 0.3f));*/
    }

    private Color GetColor(float temp)
    {
        float distFromCenter = Mathf.Abs(temp - 0.5f); // 0 when temp = 0.5
        float maxDist = 0.2f; // max possible distance from 0.5
        float t = distFromCenter / maxDist; // 0 at center, 1 at edges

        // More green at center (t = 0), more red at edges (t = 1)
        float red = t;
        float green = 1 - t;

        return new Color(red, green, 0, 0.3f);
    }



    private void UpdateRegularTemp()
    {
        SetSpriteTemp();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //SetSquareCondition();

                //SetFloatingSquareCondition();

                //SetFloatingSquareCondition(25, 25, 10, 10, 0.25f);

                //SetFloatingSquareCondition(75, 75, 4, 4, 1f);

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
