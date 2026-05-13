using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("Elementos da Tela")]
    public Image fundoMapa;
    public Image retratoP1;
    public Image retratoP2;
    public TextMeshProUGUI textoNomeMapa;

    [Header("Bancos de Imagens")]
    public Sprite[] spritesPersonagens; // Tem que ter a MESMA ordem da tela de seleção
    public Sprite[] spritesMapas;       // Tem que ter a MESMA ordem da tela de mapas
    public string[] nomesMapas;

    [Header("Configuração")]
    public float tempoDeLoading = 3.5f; // Quantos segundos a tela vai ficar aparecendo
    public string nomeCenaLuta = "CenaDeLuta"; // COLOQUE AQUI O NOME DA SUA CENA DE LUTA!

    void Start()
    {
        // 1. Resgata os números (IDs) que salvamos nas telas anteriores
        int idP1 = PlayerPrefs.GetInt("P1_Personagem_ID", 0);
        int idP2 = PlayerPrefs.GetInt("P2_Personagem_ID", 0);
        int idMapa = PlayerPrefs.GetInt("Mapa_Escolhido_ID", 0);

        // 2. Troca as imagens na tela usando os IDs
        retratoP1.sprite = spritesPersonagens[idP1];
        retratoP2.sprite = spritesPersonagens[idP2];
        fundoMapa.sprite = spritesMapas[idMapa];
        
        if (textoNomeMapa != null) 
        {
            textoNomeMapa.text = "LOCAL: " + nomesMapas[idMapa];
        }

        Invoke("IrParaLuta", tempoDeLoading);
    }

    void IrParaLuta()
    {

        SceneManager.LoadScene(nomeCenaLuta);
    }
}