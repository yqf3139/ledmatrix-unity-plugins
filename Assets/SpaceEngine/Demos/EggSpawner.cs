
using UnityEngine;
using System.Collections;

public class EggSpawner : MonoBehaviour
{
    public Transform eggPrefab;

    private float nextEggTime = 0.0f;
    private float spawnRate = 2f;

    void Update()
    {
        if (nextEggTime < Time.time)
        {
            SpawnEgg();
            nextEggTime = Time.time + spawnRate;
            spawnRate = Mathf.Clamp(spawnRate, 0.3f, 99f);
        }
    }

    void SpawnEgg()
    {
        KinectManager manager = KinectManager.Instance;

        if (eggPrefab && manager && manager.IsInitialized() && manager.IsUserDetected())
        {
            long userId = manager.GetPrimaryUserID();
            //Vector3 posUser = manager.GetUserPosition(userId);

            Vector3 spawnPos = new Vector3(Random.Range(-10, 10), 25, Random.Range(-5, 8));

            Transform eggTransform = Instantiate(eggPrefab, spawnPos, Quaternion.identity) as Transform;
            eggTransform.parent = transform;
        }
    }

}
