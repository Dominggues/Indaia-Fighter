using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Precisamos disso para mexer na fonte!

public class MenuManager : MonoBehaviour
{
    [Header("Textos do Menu")]
    public TextMeshProUGUI[] opcoesMenu; // Arraste os textos Iniciar e Creditos para cá
    
    [Header("Configuracoes Visuais")]
    public float tamanhoNormal = 50f;
    public float tamanhoSelecionado = 70f;
    public Color corNormal = Color.white;
    public Color corSelecionada = Color.yellow; 

    private int indiceAtual = 0; 

    void Start()
    {
        AtualizarVisual(); //
    }

    void Update()
    {
        // Navegar para Cima (Seta Cima ou W)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MudarOpcao(-1);
        }
        // Navegar para Baixo (Seta Baixo ou S)
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MudarOpcao(1);
        }

        // Confirmar (Enter ou Espaco)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmarSelecao();
        }
    }

    void MudarOpcao(int direcao)
    {
        indiceAtual += direcao;

        // Se passar do limite para baixo, volta pro topo
        if (indiceAtual >= opcoesMenu.Length)
        {
            indiceAtual = 0;
        }
        // Se passar do limite para cima, vai pro final
        else if (indiceAtual < 0)
        {
            indiceAtual = opcoesMenu.Length - 1;
        }

        AtualizarVisual();
    }

    void AtualizarVisual()
    {
        for (int i = 0; i < opcoesMenu.Length; i++)
        {
            if (i == indiceAtual) // Opção selecionada no momento
            {
                opcoesMenu[i].fontSize = tamanhoSelecionado;
                opcoesMenu[i].color = corSelecionada;
            }
            else // Outras opções
            {
                opcoesMenu[i].fontSize = tamanhoNormal;
                opcoesMenu[i].color = corNormal;
            }
        }
    }

    void ConfirmarSelecao()
    {
        if (indiceAtual == 0) // Se estiver no "INICIAR"
        {
            Debug.Log("Indo para a Seleção de Personagens!");
            SceneManager.LoadScene("SelecaoDePersonagem"); // Confirme se o nome está exato!
        }
        else if (indiceAtual == 1) // Se estiver nos "CREDITOS"
        {
            Debug.Log("Tela de Créditos (Ainda vamos fazer!)");
        }
    }
}