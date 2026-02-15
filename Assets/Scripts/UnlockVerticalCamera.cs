using UnityEngine;

public class UnlockVerticalCamera : MonoBehaviour
{
    [Header("Configuració")]
    [Tooltip("Límit cap amunt (graus)")]
    public float maxLookUp = 80f;

    [Tooltip("Límit cap avall (graus)")]
    public float maxLookDown = -80f;

    void Start()
    {
        Debug.Log("UnlockVerticalCamera activat");

        // Buscar el script de Starter Assets si existeix
        var starterScript = GetComponentInParent<MonoBehaviour>();
        if (starterScript != null)
        {
            var scriptType = starterScript.GetType();
            if (scriptType.Name.Contains("FirstPerson") || scriptType.Name.Contains("ThirdPerson"))
            {
                Debug.Log($"Trobat script de Starter Assets: {scriptType.Name}");

                // Intentar modificar els límits
                try
                {
                    var topClampField = scriptType.GetField("TopClamp");
                    var bottomClampField = scriptType.GetField("BottomClamp");

                    if (topClampField != null)
                    {
                        topClampField.SetValue(starterScript, maxLookUp);
                        Debug.Log($"TopClamp ajustat a {maxLookUp}");
                    }

                    if (bottomClampField != null)
                    {
                        bottomClampField.SetValue(starterScript, -maxLookDown);
                        Debug.Log($"BottomClamp ajustat a {-maxLookDown}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"No s'han pogut ajustar els clamps automàticament: {e.Message}");
                    Debug.Log("Ajusta'ls manualment a l'Inspector del script FirstPersonController");
                }
            }
        }
    }
}
