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
        if (painelPause != null) painelPause.SetActive(false);
    }

    void Update()
    {
        // Esc (teclado) OU Options de qualquer um dos dois controles PS4
        int j1 = GerenciadorControles.JoystickP1;
        int j2 = GerenciadorControles.JoystickP2;
        bool pressionouPause = Input.GetKeyDown(KeyCode.Escape)
                            || Input.GetKeyDown($"joystick {j1} button 9")
                            || Input.GetKeyDown($"joystick {j2} button 9");

        if (pressionouPause)
        {
            if (jogoPausado) AlternarPause(false);
            else             AlternarPause(true);
        }

        // Mantém o foco no botão quando estiver pausado
        if (jogoPausado
            && EventSystem.current != null
            && EventSystem.current.currentSelectedGameObject == null
            && botaoPrimeiroFoco != null)
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
            Time.timeScale = 0f;

            if (botaoPrimeiroFoco != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(botaoPrimeiroFoco.gameObject);
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void SairParaOMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
