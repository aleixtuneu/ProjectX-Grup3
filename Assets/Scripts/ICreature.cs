using UnityEngine;

public interface ICreature
{
    void SetSpawner(SequentialSpawnerBehaviour spawner);
    void OnHealthDepleted();
}