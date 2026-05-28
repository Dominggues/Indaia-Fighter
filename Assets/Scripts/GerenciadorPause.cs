using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GerenciadorPause : MonoBehaviour
{
    [Header("UI do Pause")]
    [SerializeField] private GameObject painelPause;
    [SerializeField] private Button botaoPrimeiroFoco;

    private bool jogoPausado = false;
    private string quemPausou = ""; // Guarda quem apertou o Start (P1 ou P2)
    private StandaloneInputModule inputModuleUI; // Controla quem mexe no menu

    void Start()
    {
        // Garante que o pause começa desligado ao iniciar a luta
        if (painelPause != null) painelPause.SetActive(false);

        // Acha o gerenciador de inputs da UI na cena (geralmente fica no objeto EventSystem)
        inputModuleUI = FindObjectOfType<StandaloneInputModule>();
    }

    void Update()
    {
        // --- JOGO RODANDO NORMALMENTE ---
        if (!jogoPausado)
        {
            if (Input.GetButtonDown("Start_P1")) AlternarPause(true, "P1");
            else if (Input.GetButtonDown("Start_P2")) AlternarPause(true, "P2");
            // Mantive o Esc do teclado como um "Start do P1" para facilitar testes no PC
            else if (Input.GetKeyDown(KeyCode.Escape)) AlternarPause(true, "P1");
        }
        // --- JOGO PAUSADO ---
        else
        {
            // Apenas quem pausou pode despausar pelo controle
            if (quemPausou == "P1" && (Input.GetButtonDown("Start_P1") || Input.GetKeyDown(KeyCode.Escape)))
            {
                AlternarPause(false, "");
            }
            else if (quemPausou == "P2" && Input.GetButtonDown("Start_P2"))
            {
                AlternarPause(false, "");
            }

            // Se o jogo estiver pausado e perder o foco do teclado por causa do mouse, força de volta
            if (EventSystem.current.currentSelectedGameObject == null && botaoPrimeiroFoco != null)
            {
                EventSystem.current.SetSelectedGameObject(botaoPrimeiroFoco.gameObject);
            }
        }
    }

    // Agora a função precisa saber QUEM pediu para pausar
    public void AlternarPause(bool pausar, string jogador)
    {
        jogoPausado = pausar;
        painelPause.SetActive(pausar);

        if (pausar)
        {
            quemPausou = jogador; // Salva quem foi
            Time.timeScale = 0f; // Congela o tempo do jogo
            
            // --- A MÁGICA DE TRAVAR O CONTROLE ---
            // Troca o controle do menu APENAS para quem pausou
            if (inputModuleUI != null)
            {
                inputModuleUI.horizontalAxis = "Horizontal_" + jogador;
                inputModuleUI.verticalAxis = "Vertical_" + jogador;
                inputModuleUI.submitButton = "Soco_" + jogador; // Soco confirma
                inputModuleUI.cancelButton = "Chute_" + jogador; // Chute volta/cancela
            }

            // Força o teclado/controle a focar no botão Continuar
            if (botaoPrimeiroFoco != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(botaoPrimeiroFoco.gameObject);
            }
        }
        else
        {
            quemPausou = "";
            Time.timeScale = 1f; // Devolve o tempo ao normal

            // Devolve os nomes padrões da Unity para não quebrar outros menus se o jogador sair
            if (inputModuleUI != null)
            {
                inputModuleUI.horizontalAxis = "Horizontal";
                inputModuleUI.verticalAxis = "Vertical";
                inputModuleUI.submitButton = "Submit";
                inputModuleUI.cancelButton = "Cancel";
            }
        }
    }

    // Use essa função no evento "On Click" do botão de Continuar na sua interface!
    public void BotaoDespausar()
    {
        AlternarPause(false, "");
    }

    public void SairParaOMenu()
    {
        Time.timeScale = 1f; // IMPORTANTE: Descongela o tempo antes de mudar de cena!
        
        // Também devolvemos o controle padrão antes de ir pro Menu Principal, 
        // senão o Menu Principal ficaria esperando "Soco_P1" para clicar nos botões!
        if (inputModuleUI != null)
        {
            inputModuleUI.horizontalAxis = "Horizontal";
            inputModuleUI.verticalAxis = "Vertical";
            inputModuleUI.submitButton = "Submit";
            inputModuleUI.cancelButton = "Cancel";
        }

        SceneManager.LoadScene("MenuPrincipal");
    }
}