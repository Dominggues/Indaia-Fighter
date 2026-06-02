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

    // Anti-repetição para o eixo horizontal do controle
    private bool eixoUsado = false;

    void Start()
    {
        if (spritesMapas.Length > 0)
            AtualizarInterface();
    }

    void Update()
    {
        // ─── Navegar pelos mapas ─────────────────────────────────────
        // Teclado (mantido)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MudarMapa(1);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  MudarMapa(-1);

        // Controle — D-pad ou analógico esquerdo (qualquer um dos dois controles)
        float lxP1    = Input.GetAxisRaw("PS4_LX_P1");
        float dpadXP1 = Input.GetAxisRaw("PS4_DpadX_P1");
        float lxP2    = Input.GetAxisRaw("PS4_LX_P2");
        float dpadXP2 = Input.GetAxisRaw("PS4_DpadX_P2");

        // Usa o valor absoluto maior entre analógico e D-pad de ambos os jogadores
        float hP1 = (Mathf.Abs(lxP1) > Mathf.Abs(dpadXP1)) ? lxP1 : dpadXP1;
        float hP2 = (Mathf.Abs(lxP2) > Mathf.Abs(dpadXP2)) ? lxP2 : dpadXP2;
        float eixoH = (Mathf.Abs(hP1) > Mathf.Abs(hP2)) ? hP1 : hP2;

        if (Mathf.Abs(eixoH) > 0.5f)
        {
            if (!eixoUsado)
            {
                MudarMapa(eixoH > 0 ? 1 : -1);
                eixoUsado = true;
            }
        }
        else { eixoUsado = false; }

        // ─── Confirmar mapa ──────────────────────────────────────────
        // Teclado: Space ou Enter  |  Controle: X (button 1) de qualquer jogador
        bool confirmar = Input.GetKeyDown(KeyCode.Space)
                      || Input.GetKeyDown(KeyCode.Return)
                      || Input.GetKeyDown($"joystick {GerenciadorControles.JoystickP1} button 1")
                      || Input.GetKeyDown($"joystick {GerenciadorControles.JoystickP2} button 1");
        if (confirmar)
        {
            PlayerPrefs.SetInt("Mapa_Escolhido_ID", indiceAtual);
            SceneManager.LoadScene("LoadingVersus");
        }

        // ─── Voltar para Seleção de Personagens ──────────────────────
        // Teclado: Escape  |  Controle: Círculo (button 2) de qualquer jogador
        bool voltar = Input.GetKeyDown(KeyCode.Escape)
                   || Input.GetKeyDown($"joystick {GerenciadorControles.JoystickP1} button 2")
                   || Input.GetKeyDown($"joystick {GerenciadorControles.JoystickP2} button 2");
        if (voltar)
        {
            SceneManager.LoadScene("SelecaoDePersonagem");
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
        if (displayText   != null) displayText.text = nomesMapas[indiceAtual];
    }
}
