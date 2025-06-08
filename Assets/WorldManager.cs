using UnityEngine;

public class WorldManager : MonoBehaviour
{

    [SerializeField] GameObject SpritePrefab;

    [SerializeField] GameObject VentPrefab;

    [SerializeField] GameObject Fridge;

    [SerializeField] int SpriteCount;

    [SerializeField] int VentCount;

    [SerializeField] int FridgeWidth;

    [SerializeField] int FridgeHeight;


    private Vector3 GenRandomPos ()
    {
        int minX = -FridgeWidth / 2 + 4;
        int maxX = FridgeWidth / 2 - 4;
        int minY = -FridgeHeight / 2 + 4;
        int maxY = FridgeHeight / 2 - 4;

        Vector3 pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), -1);

        for (int j = 0; j < Fridge.transform.childCount; j++)
        {
            Transform child = Fridge.transform.GetChild(j);

            Vector3 dist = child.position - pos;

            if (dist.magnitude < 20)
                return GenRandomPos();
        }

        return pos;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int minX = -FridgeWidth / 2 + 4;
        int maxX = FridgeWidth / 2 - 4;
        int minY = -FridgeHeight / 2 + 4;
        int maxY = FridgeHeight / 2 - 4;




        for (int i = 0; i < VentCount; i++)
        {
            GameObject newVent = GameObject.Instantiate(VentPrefab, Fridge.transform);

            newVent.transform.position = GenRandomPos();
        }



        for (int i = 0; i < SpriteCount; i++)
        {
            GameObject newSprite = GameObject.Instantiate(SpritePrefab, Fridge.transform);

            newSprite.GetComponent<TemperatureSprite>().SetColdSprite(true);
        }

        for (int i = 0; i < SpriteCount; i++)
        {
            GameObject newSprite = GameObject.Instantiate(SpritePrefab, Fridge.transform);

            newSprite.GetComponent<TemperatureSprite>().SetColdSprite(false);
        }

        

        FridgeTemperature fridgeTemp = Fridge.GetComponent<FridgeTemperature>();

        fridgeTemp.ChildCount = Fridge.transform.childCount;
        fridgeTemp.UpdateChildren();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
