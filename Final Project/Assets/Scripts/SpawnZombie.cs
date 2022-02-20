using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnZombie : MonoBehaviour
{
    public GameObject theZombie;
    public int xPos;
    public int zPos;
    public int zombieCount;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ZombieSpawn());
    }

    IEnumerator ZombieSpawn()
    {
        while(zombieCount < 3)
        {
            xPos = Random.Range(1, 30);
            zPos = Random.Range(1, 3);
            Instantiate(theZombie, new Vector3(xPos, 0, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            zombieCount += 1;
        }

        
    }
}
