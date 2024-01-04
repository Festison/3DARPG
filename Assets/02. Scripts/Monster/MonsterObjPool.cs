using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MonsterObjPool : SingleTon<MonsterObjPool>
{
	[Serializable]
	public class Pool
	{
		public string name;
		public GameObject prefab;
		public int size;
	}

	[SerializeField] private Pool[] pools;
	private Dictionary<string, Queue<GameObject>> poolDictionary;

	private void Start()
	{
		poolDictionary = new Dictionary<string, Queue<GameObject>>();

		AddPool();
	}

	public void AddPool()
    {
		foreach (Pool pool in pools)
		{
			Queue<GameObject> objectPool = new Queue<GameObject>();


			for (int i = 0; i < pool.size; i++)
			{
				GameObject monster = Instantiate(pool.prefab);
				monster.SetActive(false);
				objectPool.Enqueue(monster);
			}

			poolDictionary.Add(pool.name, objectPool);
		}
	}

	public GameObject PopMonster(string name, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(name))
        {
			return null;
        }

		GameObject monster = poolDictionary[name].Dequeue();

		monster.SetActive(true);
		monster.transform.position = position;
		monster.transform.rotation = rotation;

		poolDictionary[name].Enqueue(monster);

		return monster;
    }
}