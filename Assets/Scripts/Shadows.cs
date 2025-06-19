using UnityEngine;

public class LightOptimizer : MonoBehaviour
{
    [ContextMenu("Desativar sombras em luzes secundárias")]
    void Otimizar()
    {
        int contagem = 0;

        foreach (var light in FindObjectsByType<Light>(FindObjectsSortMode.None))
        {
            if (light != null && light.type != LightType.Directional)
            {
                if (light.shadows != LightShadows.None)
                {
                    light.shadows = LightShadows.None;
                    contagem++;
                }
            }
        }

        Debug.Log($"Optimizadas {contagem} luzes: sombras desativadas.");
    }
}
