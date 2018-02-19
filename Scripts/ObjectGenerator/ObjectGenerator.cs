using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unit;
using Fire;

public class ObjectGenerator : MonoBehaviour
{
    [Header("Borders Data")]
    [SerializeField]
    Vector2 bordersTerritories = new Vector2(30, 50);
    [SerializeField] float heightStartObject = 50;
    [SerializeField] float heighGeneration = 500;
    [Header("Generation Object")]
    [SerializeField]
    Transform myObject;
    [SerializeField] List<GameObject> prefabs;
    [SerializeField] GameObject prefabsWeapon;
    [SerializeField] int maxOneObject = 15;

    List<IDataUnit> allObject = new List<IDataUnit>();
    List<IDataUnit> allWeapon = new List<IDataUnit>();

    public static int nowObject = 0;
    public static int maxObject = 10;
    // Use this for initialization
    void Start()
    {
        StartCreate();
        Shuffle();
        CreateAll();
        GameTime.AddEvent(() => CreateUnit(), GameTimeController.GameTimeController.TimeEnum.second);
    }

    void StartCreate()
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            for (int j = 0; j < maxOneObject; j++)
            {
                GameObject tempObject = Instantiate(prefabs[i]);
                tempObject.SetActive(false);
                allObject.Add(tempObject.GetComponent<IDataUnit>());
            }
        }
    }

    void CreateAll()
    {
        for (int i = 0; i < ObjectGenerator.maxObject; i++)
        {
            CreateUnit();
        }
    }

    void Shuffle()
    {
        for (int i = 0; i < allObject.Count; i++)
        {
            int j = Random.Range(0, allObject.Count - 1);
            var temp = allObject[j];
            allObject[j] = allObject[i];
            allObject[i] = temp;
        }
    }

    void CreateUnit()
    {
        Vector3 startTarget = myObject.position;
        startTarget.y += heighGeneration;

        if (Random.Range(-10, 10) <= 0)
        {
            startTarget.x += Random.Range(bordersTerritories.x, bordersTerritories.y) * -1;
        }
        else
        {
            startTarget.x += Random.Range(bordersTerritories.x, bordersTerritories.y);
        }

        if (Random.Range(-10, 10) <= 0)
        {
            startTarget.z += Random.Range(bordersTerritories.x, bordersTerritories.y) * -1;
        }
        else
        {
            startTarget.z += Random.Range(bordersTerritories.x, bordersTerritories.y);
        }

        RaycastHit[] raycastHit = Physics.RaycastAll(startTarget, Vector3.down);

        for (int i = 0; i < raycastHit.Length; i++)
        {
            if (raycastHit[i].collider.GetComponent<Terrain>())
            {
                ActiveUnit(raycastHit[i].point);

            }
        }
    }

    void ActiveUnit(Vector3 position)
    {

        if (ObjectGenerator.nowObject < ObjectGenerator.maxObject)
        {
            for (int i = 0; i < allObject.Count; i++)
            {
                if (!allObject[i].isLife)
                {
                    position.y += heightStartObject;
                    allObject[i].ActionUnit_Start(position);
                    allObject.Add(allObject[i]);
                    allObject.Remove(allObject[i]);
                    ObjectGenerator.nowObject++;
                    break;
                }
            }
        }
    }
}
