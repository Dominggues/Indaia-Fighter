using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GerenciadorNavegacaoUI : MonoBehaviour
{
    [Tooltip("Botão que deve ter o foco inicial/padrão da tela")]
    public GameObject botaoFocoPadrao;

    private bool dpadVUsado = false;
    private bool dpadHUsado = false;
    private GameObject ultimoSelecionado;

    void Start()
    {
        if (botaoFocoPadrao != null)
            SelecionarBotao(botaoFocoPadrao);
    }

    void Update()
    {
        int j1 = GerenciadorControles.JoystickP1;
        int j2 = GerenciadorControles.JoystickP2;

        // Lê D-pad de ambos — usa o que tiver valor mais forte
        float dpadYP1 = Input.GetAxisRaw("PS4_DpadY_P1");
        float dpadYP2 = Input.GetAxisRaw("PS4_DpadY_P2");
        float dpadXP1 = Input.GetAxisRaw("PS4_DpadX_P1");
        float dpadXP2 = Input.GetAxisRaw("PS4_DpadX_P2");

        float dpadY = Mathf.Abs(dpadYP1) >= Mathf.Abs(dpadYP2) ? dpadYP1 : dpadYP2;
        float dpadX = Mathf.Abs(dpadXP1) >= Mathf.Abs(dpadXP2) ? dpadXP1 : dpadXP2;

        bool usouControle = false;

        // Navegação vertical
        if (Mathf.Abs(dpadY) > 0.5f)
        {
            if (!dpadVUsado)
            {
                NavegaUI(dpadY > 0 ? MoveDirection.Down : MoveDirection.Up);
                dpadVUsado = true;
                usouControle = true;
            }
        }
        else { dpadVUsado = false; }

        // Navegação horizontal
        if (Mathf.Abs(dpadX) > 0.5f)
        {
            if (!dpadHUsado)
            {
                NavegaUI(dpadX > 0 ? MoveDirection.Right : MoveDirection.Left);
                dpadHUsado = true;
                usouControle = true;
            }
        }
        else { dpadHUsado = false; }

        // Detecta qualquer input de controle para recuperar foco
        float lyP1 = Input.GetAxisRaw("PS4_LY_P1");
        float lxP1 = Input.GetAxisRaw("PS4_LX_P1");
        float lyP2 = Input.GetAxisRaw("PS4_LY_P2");
        float lxP2 = Input.GetAxisRaw("PS4_LX_P2");

        bool analogicoMoveu = Mathf.Abs(lxP1) > 0.3f || Mathf.Abs(lyP1) > 0.3f
                           || Mathf.Abs(lxP2) > 0.3f || Mathf.Abs(lyP2) > 0.3f;

        bool botaoPressionado =
            Input.GetKeyDown($"joystick {j1} button 0") ||
            Input.GetKeyDown($"joystick {j1} button 1") ||
            Input.GetKeyDown($"joystick {j1} button 2") ||
            Input.GetKeyDown($"joystick {j2} button 0") ||
            Input.GetKeyDown($"joystick {j2} button 1") ||
            Input.GetKeyDown($"joystick {j2} button 2");

        if (usouControle || analogicoMoveu || botaoPressionado)
        {
            if (EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject == null)
            {
                GameObject alvo = ultimoSelecionado != null ? ultimoSelecionado : botaoFocoPadrao;
                if (alvo != null) SelecionarBotao(alvo);
            }
        }

        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null)
        {
            ultimoSelecionado = EventSystem.current.currentSelectedGameObject;
        }
    }

    void NavegaUI(MoveDirection direcao)
    {
        if (EventSystem.current == null) return;

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            GameObject alvo = ultimoSelecionado != null ? ultimoSelecionado : botaoFocoPadrao;
            if (alvo != null) SelecionarBotao(alvo);
            return;
        }

        AxisEventData axisData = new AxisEventData(EventSystem.current);
        axisData.moveDir = direcao;
        ExecuteEvents.Execute(
            EventSystem.current.currentSelectedGameObject,
            axisData,
            ExecuteEvents.moveHandler);
    }

    void SelecionarBotao(GameObject botao)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(botao);
        ultimoSelecionado = botao;
    }
}
