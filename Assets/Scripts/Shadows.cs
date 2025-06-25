using UnityEngine;

public class LightOptimizer : MonoBehaviour
{
    void Start()
    {
        foreach (var light in FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (light.type != LightType.Directional)
            {
                light.shadows = LightShadows.None;
            }
        }
    }
}