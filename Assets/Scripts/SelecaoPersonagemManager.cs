using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SelecaoPersonagemManager : MonoBehaviour
{
    [Header("Configuração da Grade (3x3)")]
    public RectTransform[] botoesPersonagens;
    public Sprite[] artesGrandes;
    public string[] nomesPersonagens;

    [Header("Player 1")]
    public RectTransform cursorP1;
    public Image imagemPreviewP1;
    public TextMeshProUGUI textoNomeP1;
    public bool p1Pronto = false;
    private int indexP1 = 0;

    [Header("Player 2")]
    public RectTransform cursorP2;
    public Image imagemPreviewP2;
    public TextMeshProUGUI textoNomeP2;
    public bool p2Ativo = false;
    public bool p2Pronto = false;
    private int indexP2 = 2;

    [Header("Aviso P2 (Here Comes a New Challenger!)")]
    public TextMeshProUGUI textoAvisoP2;
    public float velocidadePiscar = 3f;

    private bool posicoesIniciaisAjustadas = false;

    // Variáveis para evitar que o cursor "voe" muito rápido com o analógico/setas
    private bool p1AxisH_EmUso = false;
    private bool p1AxisV_EmUso = false;
    private bool p2AxisH_EmUso = false;
    private bool p2AxisV_EmUso = false;

    void Start()
    {
        cursorP2.gameObject.SetActive(false);
        imagemPreviewP2.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        if (textoAvisoP2 != null) textoAvisoP2.gameObject.SetActive(true);
        if (textoNomeP2 != null) textoNomeP2.text = "???";

        Invoke("ForcarPosicaoInicial", 0.1f);
    }

    void ForcarPosicaoInicial()
    {
        AtualizarTelaP1();
    }

    void Update()
    {
        if (!posicoesIniciaisAjustadas)
        {
            AtualizarTelaP1();
            posicoesIniciaisAjustadas = true;
        }

        // Piscar o texto do "Here Comes a New Challenger"
        if (!p2Ativo && textoAvisoP2 != null)
        {
            Color cor = textoAvisoP2.color;
            cor.a = Mathf.PingPong(Time.time * velocidadePiscar, 1f);
            textoAvisoP2.color = cor;
        }

        // --- CONTROLES PLAYER 1 ---
        if (!p1Pronto)
        {
            // Movimentação Vertical P1
            float v1 = Input.GetAxisRaw("Vertical_P1");
            if (v1 > 0.5f && !p1AxisV_EmUso) { MoverP1(-3); p1AxisV_EmUso = true; } // Cima
            else if (v1 < -0.5f && !p1AxisV_EmUso) { MoverP1(3); p1AxisV_EmUso = true; } // Baixo
            else if (v1 > -0.5f && v1 < 0.5f) { p1AxisV_EmUso = false; }

            // Movimentação Horizontal P1
            float h1 = Input.GetAxisRaw("Horizontal_P1");
            if (h1 > 0.5f && !p1AxisH_EmUso) { MoverP1(1); p1AxisH_EmUso = true; } // Direita
            else if (h1 < -0.5f && !p1AxisH_EmUso) { MoverP1(-1); p1AxisH_EmUso = true; } // Esquerda
            else if (h1 > -0.5f && h1 < 0.5f) { p1AxisH_EmUso = false; }

            // P1 confirma com o botão de Soco
            if (Input.GetButtonDown("Soco_P1"))
            {
                p1Pronto = true;
                imagemPreviewP1.color = Color.gray;
                ChecarAmbosProntos();
            }
        }
        else 
        {
            // P1 cancela com o botão de Chute
            if (Input.GetButtonDown("Chute_P1"))
            {
                p1Pronto = false;
                imagemPreviewP1.color = Color.white;
            }
        }

        // --- CONTROLES PLAYER 2 ---
        if (!p2Ativo)
        {
            // P2 entra no jogo apertando o START (Options no PS4)
            if (Input.GetButtonDown("Start_P2"))
            {
                p2Ativo = true;
                cursorP2.gameObject.SetActive(true);
                imagemPreviewP2.color = Color.white;

                if (textoAvisoP2 != null) textoAvisoP2.gameObject.SetActive(false);

                AtualizarTelaP2();
            }
        }
        else if (!p2Pronto)
        {
            // Movimentação Vertical P2
            float v2 = Input.GetAxisRaw("Vertical_P2");
            if (v2 > 0.5f && !p2AxisV_EmUso) { MoverP2(-3); p2AxisV_EmUso = true; } 
            else if (v2 < -0.5f && !p2AxisV_EmUso) { MoverP2(3); p2AxisV_EmUso = true; } 
            else if (v2 > -0.5f && v2 < 0.5f) { p2AxisV_EmUso = false; }

            // Movimentação Horizontal P2
            float h2 = Input.GetAxisRaw("Horizontal_P2");
            if (h2 > 0.5f && !p2AxisH_EmUso) { MoverP2(1); p2AxisH_EmUso = true; } 
            else if (h2 < -0.5f && !p2AxisH_EmUso) { MoverP2(-1); p2AxisH_EmUso = true; } 
            else if (h2 > -0.5f && h2 < 0.5f) { p2AxisH_EmUso = false; }

            // P2 confirma com o botão de Soco
            if (Input.GetButtonDown("Soco_P2"))
            {
                p2Pronto = true;
                imagemPreviewP2.color = Color.gray;
                ChecarAmbosProntos();
            }
        }
        else 
        {
            // P2 cancela com o botão de Chute
            if (Input.GetButtonDown("Chute_P2"))
            {
                p2Pronto = false;
                imagemPreviewP2.color = Color.white;
            }
        }

        // --- VOLTAR PARA O MENU PRINCIPAL ---
        // Mantive o Escape do teclado como segurança
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
    }

    void MoverP1(int direcao)
    {
        int novoIndex = indexP1 + direcao;
        if (novoIndex >= 0 && novoIndex < botoesPersonagens.Length)
        {
            if (direcao == 1 && indexP1 % 3 == 2) return;
            if (direcao == -1 && indexP1 % 3 == 0) return;

            indexP1 = novoIndex;
            AtualizarTelaP1();
        }
    }

    void AtualizarTelaP1()
    {
        cursorP1.position = botoesPersonagens[indexP1].position;
        imagemPreviewP1.sprite = artesGrandes[indexP1];
        if (textoNomeP1 != null) textoNomeP1.text = nomesPersonagens[indexP1];
    }

    void MoverP2(int direcao)
    {
        int novoIndex = indexP2 + direcao;
        if (novoIndex >= 0 && novoIndex < botoesPersonagens.Length)
        {
            if (direcao == 1 && indexP2 % 3 == 2) return;
            if (direcao == -1 && indexP2 % 3 == 0) return;

            indexP2 = novoIndex;
            AtualizarTelaP2();
        }
    }

    void AtualizarTelaP2()
    {
        cursorP2.position = botoesPersonagens[indexP2].position;
        imagemPreviewP2.sprite = artesGrandes[indexP2];
        if (textoNomeP2 != null) textoNomeP2.text = nomesPersonagens[indexP2];
    }

    void ChecarAmbosProntos()
    {
        if (p1Pronto && p2Pronto)
        {
            PlayerPrefs.SetInt("P1_Personagem_ID", indexP1);
            PlayerPrefs.SetInt("P2_Personagem_ID", indexP2);
            SceneManager.LoadScene("SelecaoDeMapas");
        }
    }
}