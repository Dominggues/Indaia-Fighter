using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SelecaoMapaManager : MonoBehaviour
{
    [Header("Configuracoes dos Mapas")]
    public Sprite[] spritesMapas;
    public string[] nomesMapas;

    [Header("Elementos da Tela")]
    public Image displayImagem;
    public TextMeshProUGUI displayText;

    private int indiceAtual = 0;

    void Start()
    {
        if (spritesMapas.Length > 0)
        {
            AtualizarInterface();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MudarMapa(1);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MudarMapa(-1);

        // Confirmar Mapa e ir para o Loading
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Mapa Escolhido: " + nomesMapas[indiceAtual]);
            
            // Salva o ID do mapa no cérebro do jogo
            PlayerPrefs.SetInt("Mapa_Escolhido_ID", indiceAtual);
            
            // Pula para a tela de Loading
            SceneManager.LoadScene("LoadingVersus");
        }
    }

    public void MudarMapa(int direcao)
    {
        indiceAtual += direcao;
        if (indiceAtual >= spritesMapas.Length) indiceAtual = 0;
        if (indiceAtual < 0) indiceAtual = spritesMapas.Length - 1;
        AtualizarInterface();
    }

    void AtualizarInterface()
    {
        if (displayImagem != null) displayImagem.sprite = spritesMapas[indiceAtual];
        if (displayText != null) displayText.text = nomesMapas[indiceAtual];
    }
}