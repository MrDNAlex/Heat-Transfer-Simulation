using UnityEngine;

public class Vent : TempSprite
{
    public Vector3 ParentSize;
    public float stopTime;
    public float CoolDownTime;

    float[,] tempData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), -1);
        CoolDownTime = Time.time + Random.Range(5, 10);

        this.TempSize = 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (Time.time < stopTime || Time.time < CoolDownTime)
            return;
        else
           IsActive = false;

        float sum = 0;

        int posX = (int)transform.position.x + parentSizeXHalf;
        int posY = (int)transform.position.y + parentSizeYHalf;

        float tempAtVent = tempData[posX, posY];

        if (tempAtVent > 0.6f)
        {
            IsActive = true;
            stopTime = Time.time + Random.Range(3, 10);
            CoolDownTime = Time.time + Random.Range(7, 15);
            Temperature = 0.2f;
        }

        if (tempAtVent < 0.4f)
        {
            IsActive = true;
            stopTime = Time.time + Random.Range(3, 10);
            CoolDownTime = Time.time + Random.Range(7, 15);
            Temperature = 0.8f;
        }
    }

    public void SetParentSize(Vector3 size)
    {
        ParentSize = size;

        minX = -ParentSize.x / 2 + 4;
        maxX = ParentSize.x / 2 - 4;
        minY = -ParentSize.y / 2 + 4;
        maxY = ParentSize.y / 2 - 4;

        parentSizeXHalf = (int)ParentSize.x / 2;
        parentSizeYHalf = (int)ParentSize.y / 2;

       tempData = transform.parent.GetComponent<FridgeTemperature>().tempData;
    }
}
