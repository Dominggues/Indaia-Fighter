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

    void Start()
    {
        // Garante que o pause começa desligado ao iniciar a luta
        if (painelPause != null) painelPause.SetActive(false);
    }

    void Update()
    {
        // Esc ativa ou desativa o pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (jogoPausado) AlternarPause(false);
            else AlternarPause(true);
        }

        // Se o jogo estiver pausado e perder o foco do teclado por causa do mouse, força de volta
        if (jogoPausado && EventSystem.current.currentSelectedGameObject == null && botaoPrimeiroFoco != null)
        {
            EventSystem.current.SetSelectedGameObject(botaoPrimeiroFoco.gameObject);
        }
    }

    public void AlternarPause(bool pausar)
    {
        jogoPausado = pausar;
        painelPause.SetActive(pausar);

        if (pausar)
        {
            Time.timeScale = 0f; // Congela o tempo do jogo (física, animações, etc)
            
            // Força o teclado a focar no botão Continuar
            if (botaoPrimeiroFoco != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(botaoPrimeiroFoco.gameObject);
            }
        }
        else
        {
            Time.timeScale = 1f; // Devolve o tempo ao normal
        }
    }

    public void SairParaOMenu()
    {
        Time.timeScale = 1f; // IMPORTANTE: Descongela o tempo antes de mudar de cena!
        SceneManager.LoadScene("MenuPrincipal"); // Troque pelo nome exato da sua cena de menu
    }
}