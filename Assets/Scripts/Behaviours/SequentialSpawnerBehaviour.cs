using System;
using System.Collections.Generic;
using UnityEngine;

public class SequentialSpawnerBehaviour : MonoBehaviour
{
    [Serializable]
    public class CreaturePair
    {
        public GameObject creatureA;
        public GameObject creatureB;
    }

    [SerializeField] private List<CreaturePair> creaturePairs = new List<CreaturePair>();
    [SerializeField] private Transform spawnPositionA;
    [SerializeField] private Transform spawnPositionB;

    private int currentPairIndex = 0;
    private int deadCount = 0;
    private bool isActive = false;

    public void Initialize()
    {
        if (isActive)
        {
            Debug.LogWarning("Spawner already initialized!");
            return;
        }

        if (creaturePairs.Count > 0)
        {
            isActive = true;
            currentPairIndex = 0;
            deadCount = 0;
            SpawnCurrentPair();
        }
        else
        {
            Debug.LogWarning("No creature pairs assigned to spawner!");
        }
    }

    public void OnCreatureDeath()
    {
        if (!isActive) return;

        deadCount++;
        //Debug.Log($"Creature died. Dead count: {deadCount}/2");

        if (deadCount >= 2)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        deadCount = 0;
        currentPairIndex++;

        if (currentPairIndex < creaturePairs.Count)
        {
            //Debug.Log($"Spawning next wave: {currentPairIndex}");
            SpawnCurrentPair();
        }
        else
        {
            //Debug.Log("All creature pairs spawned. Disabling spawner.");
            DisableSpawner();
        }
    }

    private void SpawnCurrentPair()
    {
        CreaturePair pair = creaturePairs[currentPairIndex];

        if (pair.creatureA)
        {
            GameObject creatureA = Instantiate(pair.creatureA, spawnPositionA.position, Quaternion.identity);
            
            ICreature creatureComponentA = creatureA.GetComponent<ICreature>();
            if (creatureComponentA != null)
            {
                creatureComponentA.SetSpawner(this);
            }
            
            //Debug.Log($"Spawned Creature A: {pair.creatureA.name} at pair {currentPairIndex}");
        }

        if (pair.creatureB)
        {
            GameObject creatureB = Instantiate(pair.creatureB, spawnPositionB.position, Quaternion.identity);
            
            ICreature creatureComponentB = creatureB.GetComponent<ICreature>();
            creatureComponentB?.SetSpawner(this);

            //Debug.Log($"Spawned Creature B: {pair.creatureB.name} at pair {currentPairIndex}");
        }
    }

    private void DisableSpawner()
    {
        isActive = false;
        enabled = false;
        this.gameObject.SetActive(false);
    }
}