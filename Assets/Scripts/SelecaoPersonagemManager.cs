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
    public TextMeshProUGUI textoNomeP1; // <-- NOVO: Texto do nome do P1
    public bool p1Pronto = false;
    private int indexP1 = 0; 

    [Header("Player 2")]
    public RectTransform cursorP2;
    public Image imagemPreviewP2;
    public TextMeshProUGUI textoNomeP2; // <-- NOVO: Texto do nome do P2
    public bool p2Ativo = false;
    public bool p2Pronto = false;
    private int indexP2 = 2; 

    [Header("Aviso P2 (Here Comes a New Challenger!)")]
    public TextMeshProUGUI textoAvisoP2; 
    public float velocidadePiscar = 3f;  

    private bool posicoesIniciaisAjustadas = false; 

    void ForcarPosicaoInicial()
        {
            AtualizarTelaP1();
        }

    void Start()
    {
        cursorP2.gameObject.SetActive(false);
        imagemPreviewP2.color = new Color(0.2f, 0.2f, 0.2f, 1f); 
        
        if (textoAvisoP2 != null) textoAvisoP2.gameObject.SetActive(true);
        
        if (textoNomeP2 != null) textoNomeP2.text = "???"; 

        Invoke("ForcarPosicaoInicial", 0.1f);
    }

    void Update()
    {
        if (!posicoesIniciaisAjustadas)
        {
            AtualizarTelaP1();
            posicoesIniciaisAjustadas = true;
        }

        if (!p2Ativo && textoAvisoP2 != null)
        {
            Color cor = textoAvisoP2.color;
            cor.a = Mathf.PingPong(Time.time * velocidadePiscar, 1f);
            textoAvisoP2.color = cor;
        }

        // --- CONTROLES PLAYER 1 (WASD e Enter) ---
        if (!p1Pronto)
        {
            if (Input.GetKeyDown(KeyCode.W)) MoverP1(-3); 
            if (Input.GetKeyDown(KeyCode.S)) MoverP1(3);  
            if (Input.GetKeyDown(KeyCode.A)) MoverP1(-1); 
            if (Input.GetKeyDown(KeyCode.D)) MoverP1(1);  

            // P1 agora confirma com ENTER
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) 
            {
                p1Pronto = true;
                imagemPreviewP1.color = Color.gray; 
                ChecarAmbosProntos();
            }
        }

        // --- CONTROLES PLAYER 2 (Setinhas e M) ---
        if (!p2Ativo)
        {
            // P2 agora entra no jogo apertando M
            if (Input.GetKeyDown(KeyCode.M)) 
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
            if (Input.GetKeyDown(KeyCode.UpArrow)) MoverP2(-3);
            if (Input.GetKeyDown(KeyCode.DownArrow)) MoverP2(3);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MoverP2(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoverP2(1);

            // P2 agora confirma com M
            if (Input.GetKeyDown(KeyCode.M)) 
            {
                p2Pronto = true;
                imagemPreviewP2.color = Color.gray;
                ChecarAmbosProntos();
            }
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
        
        // Atualiza o texto com o nome do personagem P1
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
        
        // Atualiza o texto com o nome do personagem P2
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