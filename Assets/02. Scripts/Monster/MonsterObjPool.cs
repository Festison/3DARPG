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
	
	
	[Tooltip("여러 몬스터를 담기 위한 내부 클래스")] [SerializeField] private Pool[] pools;
	// 여러 몬스터를 큐에 담기 위해 다중 오브젝트 풀링을 사용할 딕셔너리
	private Dictionary<string, Queue<GameObject>> poolDictionary;

	private void Start()
	{
		poolDictionary = new Dictionary<string, Queue<GameObject>>();

		AddPool();
	}

	/// <summary>
	/// 내부 클래스에 담겨있는 몬스터들을 큐에 집어넣어 미리 객체화 시켜놓고 비활성화
	/// </summary>
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

	/// <summary>
	/// 몬스터의 이름 위치 회전 값을 받아 특정 몬스터를 활성화
	/// </summary>
	/// <param name="name"></param>
	/// <param name="position"></param>
	/// <param name="rotation"></param>
	/// <returns></returns>
	public GameObject PopMonster(string name, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(name))
			return null;

		GameObject monster = poolDictionary[name].Dequeue();

		monster.SetActive(true);
		monster.transform.position = position;
		monster.transform.rotation = rotation;

		poolDictionary[name].Enqueue(monster);

		return monster;
    }
}