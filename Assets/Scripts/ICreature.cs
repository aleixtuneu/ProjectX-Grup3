using UnityEngine;

public interface ICreature
{//h
    void SetSpawner(SequentialSpawnerBehaviour spawner);
    void OnHealthDepleted();
}