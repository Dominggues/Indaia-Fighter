using UnityEngine;

public class GerenciadorDaLuta : MonoBehaviour
{
    [Header("Lista de Cenários")]
    public GameObject[] cenariosDaCena; 

    [Header("Spawns (Onde eles vão nascer)")]
    public Transform localSpawnP1; 
    public Transform localSpawnP2; 

    [Header("Lista de Personagens Disponíveis")]
    // Aqui você vai colocar os PREFABS dos personagens na MESMA ORDEM da tela de seleção
    public GameObject[] personagensDisponiveis; 

    void Start()
    {
        CarregarMapaCorreto();
        CarregarLutadores(); // Chamamos a nova função aqui!
    }

    void CarregarMapaCorreto()
    {
        for (int i = 0; i < cenariosDaCena.Length; i++)
        {
            cenariosDaCena[i].SetActive(false);
        }

        int idMapaEscolhido = PlayerPrefs.GetInt("Mapa_Escolhido_ID", 0);

        if (idMapaEscolhido < cenariosDaCena.Length)
        {
            cenariosDaCena[idMapaEscolhido].SetActive(true);
        }
    }

    void CarregarLutadores()
    {
        // 1. Puxa da memória quem foi escolhido
        int idP1 = PlayerPrefs.GetInt("P1_Personagem_ID", 0);
        int idP2 = PlayerPrefs.GetInt("P2_Personagem_ID", 0);

        // 2. Faz o Player 1 nascer na posição do objeto P1
        if (idP1 < personagensDisponiveis.Length && personagensDisponiveis[idP1] != null)
        {
            Instantiate(personagensDisponiveis[idP1], localSpawnP1.position, Quaternion.identity);
        }

        // 3. Faz o Player 2 nascer na posição do objeto P2 e vira ele para a esquerda!
        if (idP2 < personagensDisponiveis.Length && personagensDisponiveis[idP2] != null)
        {
            GameObject lutador2 = Instantiate(personagensDisponiveis[idP2], localSpawnP2.position, Quaternion.identity);
            
            // Inverte a escala no X para ele olhar para a esquerda (encarando o P1)
            lutador2.transform.localScale = new Vector3(-3, 3, 3);
        }
    }
}