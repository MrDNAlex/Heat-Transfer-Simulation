using UnityEngine;

public class TemperatureSprite : TempSprite
{
    public bool isCold;
    public float Mag;
    public float Speed;
    public Vector3 MovementDirection;
    public float Offset;
    public Vector3 ParentSize;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        this.TempSize = 2;

        IsActive = true;

        //this.Parent = this.transform.parent;
        this.Offset = Random.Range(0, 100000f);

        if (isCold)
            Temperature = Random.Range(0.15f, 0.3f);
        else
            Temperature = Random.Range(0.7f, 0.85f);

        MovementDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));

        transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), -1);

        Speed = Random.Range(0.5f, 0.9f);
    }

    public void SetColdSprite (bool isCold)
    {
        this.isCold = isCold;
    }

    public void SetParentSize(Vector3 size)
    {
        ParentSize = size;

        minX = -ParentSize.x / 2 + 4;
        maxX = ParentSize.x / 2 - 4;
        minY = -ParentSize.y / 2 + 4 ;
        maxY = ParentSize.y / 2 - 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        float x = Mathf.PerlinNoise(Time.time * Speed, Offset) - 0.5f;
        float y = Mathf.PerlinNoise(Offset, Time.time * Speed) - 0.5f;

        MovementDirection += new Vector3(x, y, 0) * Mag;
        Vector3 futurePosition = transform.position + MovementDirection * Speed;

        if (MovementDirection.magnitude > 1)
            MovementDirection.Normalize();

        // Bounce and clamp
        if (futurePosition.x < minX || futurePosition.x > maxX)
        {
            MovementDirection.x *= -1;
            futurePosition.x = Mathf.Clamp(futurePosition.x, minX, maxX);
            Offset = Random.Range(0, 100000f);
        }
        if (futurePosition.y < minY || futurePosition.y > maxY)
        {
            MovementDirection.y *= -1;
            futurePosition.y = Mathf.Clamp(futurePosition.y, minY, maxY);
            Offset = Random.Range(0, 100000f);
        }

        transform.position = futurePosition;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector3 dir = collision.transform.position - transform.position;

        dir.z = 0;
        //if (collision.gameObject.name == "Fridge")
        //{
        //    Debug.Log($"Hitting Fridge, adding Dir : {dir}");
        //    dir *= -1;
        //}


        dir.Normalize();

        MovementDirection += dir * -1;

        MovementDirection.Normalize();
    }



    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log($"Colliding With {collision.gameObject.name}");

    //    Vector3 dir = collision.transform.position - transform.position;

    //    dir.Normalize();

    //    if (collision.gameObject.name == "Fridge")

    //        MovementDirection += dir;
    //    else
    //        MovementDirection += dir * -1;

    //    MovementDirection.Normalize();
    //}

   
}
