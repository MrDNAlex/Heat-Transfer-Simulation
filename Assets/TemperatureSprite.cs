using UnityEngine;

public class TemperatureSprite : MonoBehaviour
{
    public float Mag;
    public float Speed;
    public float Temperature;
    public Vector3 MovementDirection;
    public Transform Parent;
    public float Offset;
    public Vector3 StartPos;
    public Vector3 ParentSize;
    private bool SizeSet;


    float minX;
    float maxX;
    float minY;
    float maxY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Parent = this.transform.parent;
        this.Offset = Random.Range(0, 100000f);
        this.StartPos = this.transform.position;
        SizeSet = false;

        MovementDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }

    public void SetParentSize(Vector3 size)
    {
        ParentSize = size;
        SizeSet = true;

        minX = -ParentSize.x / 2 + 2;
        maxX = ParentSize.x / 2 - 2;
        minY = -ParentSize.y / 2 + 2 ;
        maxY = ParentSize.y / 2 - 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!SizeSet)
            return;

        float x = Mathf.PerlinNoise(Time.time * Speed, Offset) - 0.5f;
        float y = Mathf.PerlinNoise(Offset, Time.time * Speed) - 0.5f;

        //Vector3 ParentSize = Parent.GetComponent<SpriteRenderer>().bounds.size;
        Vector3 wander = new Vector3(x, y, 0) * Mag;

        MovementDirection += wander;
        Vector3 futurePosition = transform.position + MovementDirection;

        // Bounce and clamp
        if (futurePosition.x < minX || futurePosition.x > maxX)
        {
            MovementDirection.x *= -1;
            futurePosition.x = Mathf.Clamp(futurePosition.x, minX, maxX);
        }
        if (futurePosition.y < minY || futurePosition.y > maxY)
        {
            MovementDirection.y *= -1;
            futurePosition.y = Mathf.Clamp(futurePosition.y, minY, maxY);
        }

        if (MovementDirection.magnitude > 1)
            MovementDirection.Normalize();

        transform.position = futurePosition;


        /*if (futurePosition.x < -parentXHalfSize || futurePosition.x > parentXHalfSize)
            MovementDirection.x *= -1;

        if (futurePosition.y < -parentYHalfSize || futurePosition.y > parentYHalfSize)
            MovementDirection.y *= -1;

        MovementDirection.Normalize();

        this.transform.position += MovementDirection;*/
    }
}
