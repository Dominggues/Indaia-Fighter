using UnityEngine;

public class RolagemCreditos : MonoBehaviour
{
    [Header("Configurações")]
    public float velocidade = 50f; // Ajuste a velocidade de subida aqui
    
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Move o texto para cima de forma perfeitamente suave em qualquer framerate
        rectTransform.anchoredPosition += Vector2.up * velocidade * Time.deltaTime;
    }
}