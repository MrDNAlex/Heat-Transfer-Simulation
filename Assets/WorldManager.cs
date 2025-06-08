using UnityEngine;

public class WorldManager : MonoBehaviour
{

    [SerializeField] GameObject SpritePrefab;

    [SerializeField] GameObject Fridge;

    [SerializeField] int SpriteCount;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {




        for (int i = 0; i < SpriteCount; i++)
        {
            GameObject newSprite = GameObject.Instantiate(SpritePrefab);

            newSprite.transform.parent = Fridge.transform;

            newSprite.GetComponent<TemperatureSprite>().SetColdSprite(true);
        }

        for (int i = 0; i < SpriteCount; i++)
        {
            GameObject newSprite = GameObject.Instantiate(SpritePrefab);

            newSprite.transform.parent = Fridge.transform;

            newSprite.GetComponent<TemperatureSprite>().SetColdSprite(false);
        }

        FridgeTemperature fridgeTemp = Fridge.GetComponent<FridgeTemperature>();

        fridgeTemp.ChildCount = SpriteCount * 2;
        fridgeTemp.UpdateChildren();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
