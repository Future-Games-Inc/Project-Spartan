using System.Collections.Generic;
using UnityEngine;



public class ObjectPool
{
    private GameObject parent;
    // Object pool will have a PoolableObject Prefab variable
    private PoolableObject prefab;

    private int Size;
    // And it will have a list of poolabale object variable called availableobjects.  
    private List<PoolableObject> availableObjects;

    // private constructor that takes in a poolableObject Prefab and the in size. 
    private ObjectPool (PoolableObject Prefab, int size)
        {
        //this constructoc will make this.Prefab equal the prefab that is passed in. 
        this.prefab = Prefab;
        this.Size = size;
        //This constructor will then go ahead and make a list of (x-Size) valled available Objects. 
        availableObjects = new List<PoolableObject>(size);
        }


    public static ObjectPool CreateInstance(PoolableObject prefab, int size)
    {
        ObjectPool pool = new ObjectPool(prefab, size);

        pool.parent = new GameObject(prefab + "Pool");
        pool.CreateObjects();

        return pool;


    }

    private void CreateObjects( )
    {
        for (int i = 0; i < Size; i++)
        {
            CreateObject();
        }

    }

    private void CreateObject()
    {
        PoolableObject poolableObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
        poolableObject.Parent = this;
        poolableObject.gameObject.SetActive(false); // poolable object handles re-adding the object to the availableobjects
    }

    public void ReturnObjectToPool(PoolableObject poolableObject)
    {
        availableObjects.Add(poolableObject);

    }

    public PoolableObject GetObject()
    {
        if (availableObjects.Count == 0) //auto expand pool size if out of objects
        {
            CreateObject();


        }
            PoolableObject instance = availableObjects[0];
            availableObjects.RemoveAt(0);

            instance.gameObject.SetActive(true);

            return instance;


        
    }




}
