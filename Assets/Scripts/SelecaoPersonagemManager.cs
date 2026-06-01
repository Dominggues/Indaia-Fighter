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

    private bool eixoP1UsadoH = false;
    private bool eixoP1UsadoV = false;
    private bool eixoP2UsadoH = false;
    private bool eixoP2UsadoV = false;

    private bool posicoesIniciaisAjustadas = false;

    // Cooldown para evitar que cancelar seleção e voltar ao menu
    // sejam acionados pelo mesmo apertar de botão
    private float cooldownCirculo = 0f;
    private const float COOLDOWN_CIRCULO = 0.3f;

    void ForcarPosicaoInicial() { AtualizarTelaP1(); }

    void Start()
    {
        cursorP2.gameObject.SetActive(false);
        imagemPreviewP2.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        if (textoAvisoP2 != null) textoAvisoP2.gameObject.SetActive(true);
        if (textoNomeP2 != null)  textoNomeP2.text = "???";

        Invoke("ForcarPosicaoInicial", 0.1f);
    }

    void Update()
    {
        if (!posicoesIniciaisAjustadas)
        {
            AtualizarTelaP1();
            posicoesIniciaisAjustadas = true;
        }

        // Decrementa o cooldown do Círculo a cada frame
        if (cooldownCirculo > 0f) cooldownCirculo -= Time.deltaTime;

        if (!p2Ativo && textoAvisoP2 != null)
        {
            Color cor = textoAvisoP2.color;
            cor.a = Mathf.PingPong(Time.time * velocidadePiscar, 1f);
            textoAvisoP2.color = cor;
        }

        // ════════════════════════════════════════════════
        //  PLAYER 1
        // ════════════════════════════════════════════════
        if (!p1Pronto)
        {
            // Teclado
            if (Input.GetKeyDown(KeyCode.W)) MoverP1(-3);
            if (Input.GetKeyDown(KeyCode.S)) MoverP1(3);
            if (Input.GetKeyDown(KeyCode.A)) MoverP1(-1);
            if (Input.GetKeyDown(KeyCode.D)) MoverP1(1);

            // Controle PS4 P1 — analógico (eixos 1 e 2)
            float lxP1 = Input.GetAxisRaw("PS4_LX_P1");
            float lyP1 = Input.GetAxisRaw("PS4_LY_P1");
            // D-pad P1 — eixos 5 e 6 (correto para DS4 USB no Windows)
            float dpadXP1 = Input.GetAxisRaw("PS4_DpadX_P1");
            float dpadYP1 = Input.GetAxisRaw("PS4_DpadY_P1");

            float hP1 = (Mathf.Abs(lxP1) > Mathf.Abs(dpadXP1)) ? lxP1 : dpadXP1;
            float vP1 = (Mathf.Abs(lyP1) > Mathf.Abs(dpadYP1)) ? lyP1 : dpadYP1;

            if (Mathf.Abs(hP1) > 0.5f)
            {
                if (!eixoP1UsadoH) { MoverP1(hP1 > 0 ? 1 : -1); eixoP1UsadoH = true; }
            }
            else { eixoP1UsadoH = false; }

            if (Mathf.Abs(vP1) > 0.5f)
            {
                if (!eixoP1UsadoV) { MoverP1(vP1 > 0 ? 3 : -3); eixoP1UsadoV = true; }
            }
            else { eixoP1UsadoV = false; }

            // Confirmar — Enter ou X (button 1)
            bool confirmarP1 = Input.GetKeyDown(KeyCode.Return)
                            || Input.GetKeyDown(KeyCode.KeypadEnter)
                            || Input.GetKeyDown("joystick 1 button 1");
            if (confirmarP1)
            {
                p1Pronto = true;
                imagemPreviewP1.color = Color.gray;
                ChecarAmbosProntos();
                return; // evita processar mais inputs neste frame
            }

            // Voltar ao Menu — Escape ou Círculo (button 2), só quando P1 não está pronto
            // e somente se o cooldown já zerou (evita voltar logo após cancelar seleção)
            bool voltarMenu = (cooldownCirculo <= 0f) &&
                              (Input.GetKeyDown(KeyCode.Escape)
                           || Input.GetKeyDown("joystick 1 button 2"));
            if (voltarMenu)
            {
                SceneManager.LoadScene("MenuPrincipal");
                return;
            }
        }
        else
        {
            // P1 já confirmou — Backspace ou Círculo (button 2) cancela a seleção
            bool cancelarP1 = Input.GetKeyDown(KeyCode.Backspace)
                           || Input.GetKeyDown("joystick 1 button 2");
            if (cancelarP1)
            {
                p1Pronto = false;
                imagemPreviewP1.color = Color.white;
                cooldownCirculo = COOLDOWN_CIRCULO; // inicia o cooldown
                return;
            }
        }

        // ════════════════════════════════════════════════
        //  PLAYER 2
        // ════════════════════════════════════════════════
        if (!p2Ativo)
        {
            bool entrarP2 = Input.GetKeyDown(KeyCode.M)
                         || Input.GetKeyDown("joystick 2 button 9");
            if (entrarP2)
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
            // Teclado
            if (Input.GetKeyDown(KeyCode.UpArrow))    MoverP2(-3);
            if (Input.GetKeyDown(KeyCode.DownArrow))  MoverP2(3);
            if (Input.GetKeyDown(KeyCode.LeftArrow))  MoverP2(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) MoverP2(1);

            // Controle PS4 P2
            float lxP2    = Input.GetAxisRaw("PS4_LX_P2");
            float lyP2    = Input.GetAxisRaw("PS4_LY_P2");
            float dpadXP2 = Input.GetAxisRaw("PS4_DpadX_P2");
            float dpadYP2 = Input.GetAxisRaw("PS4_DpadY_P2");

            float hP2 = (Mathf.Abs(lxP2) > Mathf.Abs(dpadXP2)) ? lxP2 : dpadXP2;
            float vP2 = (Mathf.Abs(lyP2) > Mathf.Abs(dpadYP2)) ? lyP2 : dpadYP2;

            if (Mathf.Abs(hP2) > 0.5f)
            {
                if (!eixoP2UsadoH) { MoverP2(hP2 > 0 ? 1 : -1); eixoP2UsadoH = true; }
            }
            else { eixoP2UsadoH = false; }

            if (Mathf.Abs(vP2) > 0.5f)
            {
                if (!eixoP2UsadoV) { MoverP2(vP2 > 0 ? 3 : -3); eixoP2UsadoV = true; }
            }
            else { eixoP2UsadoV = false; }

            // Confirmar — M ou X (button 1)
            bool confirmarP2 = Input.GetKeyDown(KeyCode.M)
                            || Input.GetKeyDown("joystick 2 button 1");
            if (confirmarP2)
            {
                p2Pronto = true;
                imagemPreviewP2.color = Color.gray;
                ChecarAmbosProntos();
                return;
            }
        }
        else
        {
            // P2 confirmou — RightShift ou Círculo (button 2) cancela
            bool cancelarP2 = Input.GetKeyDown(KeyCode.RightShift)
                           || Input.GetKeyDown("joystick 2 button 2");
            if (cancelarP2)
            {
                p2Pronto = false;
                imagemPreviewP2.color = Color.white;
            }
        }
    }

    void MoverP1(int direcao)
    {
        int novoIndex = indexP1 + direcao;
        if (novoIndex >= 0 && novoIndex < botoesPersonagens.Length)
        {
            if (direcao ==  1 && indexP1 % 3 == 2) return;
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
            if (direcao ==  1 && indexP2 % 3 == 2) return;
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
