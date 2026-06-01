using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Coloca em qualquer GameObject das cenas que têm menu de UI:
/// MenuPrincipal, Pause (no mesmo objeto do GerenciadorPause),
/// e tela de Vitória (no mesmo objeto do ControladorLuta).
///
/// Resolve dois problemas:
/// 1. D-pad navega pelos botões de UI (igual ao analógico)
/// 2. Mouse não rouba o foco — ao mexer no controle o foco volta
/// </summary>
public class GerenciadorNavegacaoUI : MonoBehaviour
{
    [Tooltip("Botão que deve ter o foco inicial/padrão da tela")]
    public GameObject botaoFocoPadrao;

    // --- anti-repetição do eixo D-pad ---
    private bool dpadVUsado = false;
    private bool dpadHUsado = false;

    // Guarda o último objeto selecionado pelo controle
    private GameObject ultimoSelecionado;

    void Start()
    {
        if (botaoFocoPadrao != null)
            SelecionarBotao(botaoFocoPadrao);
    }

    void Update()
    {
        // ── Lê D-pad de ambos os jogadores ──────────────────────────
        float dpadY = Mathf.Abs(Input.GetAxisRaw("PS4_DpadY_P1")) > 0.5f
                    ? Input.GetAxisRaw("PS4_DpadY_P1")
                    : Input.GetAxisRaw("PS4_DpadY_P2");

        float dpadX = Mathf.Abs(Input.GetAxisRaw("PS4_DpadX_P1")) > 0.5f
                    ? Input.GetAxisRaw("PS4_DpadX_P1")
                    : Input.GetAxisRaw("PS4_DpadX_P2");

        bool usouControle = false;

        // ── Navegação vertical com D-pad ────────────────────────────
        if (Mathf.Abs(dpadY) > 0.5f)
        {
            if (!dpadVUsado)
            {
                // dpadY > 0 = cima (Invert marcado), envia Navigate Up
                // dpadY < 0 = baixo, envia Navigate Down
                NavegaUI(dpadY > 0
                    ? MoveDirection.Down
                    : MoveDirection.Up);
                dpadVUsado = true;
                usouControle = true;
            }
        }
        else { dpadVUsado = false; }

        // ── Navegação horizontal com D-pad ──────────────────────────
        if (Mathf.Abs(dpadX) > 0.5f)
        {
            if (!dpadHUsado)
            {
                NavegaUI(dpadX > 0
                    ? MoveDirection.Right
                    : MoveDirection.Left);
                dpadHUsado = true;
                usouControle = true;
            }
        }
        else { dpadHUsado = false; }

        // ── Detecta qualquer input de controle (analógico ou botão) ─
        // para recuperar foco se o mouse tiver roubado
        float lyP1 = Input.GetAxisRaw("PS4_LY_P1");
        float lxP1 = Input.GetAxisRaw("PS4_LX_P1");
        float lyP2 = Input.GetAxisRaw("PS4_LY_P2");
        float lxP2 = Input.GetAxisRaw("PS4_LX_P2");

        bool analogicoMoveu = Mathf.Abs(lxP1) > 0.3f || Mathf.Abs(lyP1) > 0.3f
                           || Mathf.Abs(lxP2) > 0.3f || Mathf.Abs(lyP2) > 0.3f;

        bool botaoPressionado =
            Input.GetKeyDown("joystick 1 button 0") ||
            Input.GetKeyDown("joystick 1 button 1") ||
            Input.GetKeyDown("joystick 1 button 2") ||
            Input.GetKeyDown("joystick 2 button 0") ||
            Input.GetKeyDown("joystick 2 button 1") ||
            Input.GetKeyDown("joystick 2 button 2");

        if (usouControle || analogicoMoveu || botaoPressionado)
        {
            // Se o EventSystem perdeu o foco (mouse roubou), restaura
            if (EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject == null)
            {
                GameObject alvo = ultimoSelecionado != null
                               ? ultimoSelecionado
                               : botaoFocoPadrao;

                if (alvo != null) SelecionarBotao(alvo);
            }
        }

        // Atualiza o último selecionado (só quando é pelo controle/teclado)
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null)
        {
            ultimoSelecionado = EventSystem.current.currentSelectedGameObject;
        }
    }

    // Envia um evento de navegação diretamente para o EventSystem
    void NavegaUI(MoveDirection direcao)
    {
        if (EventSystem.current == null) return;

        // Garante que há um objeto selecionado antes de navegar
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            GameObject alvo = ultimoSelecionado != null
                           ? ultimoSelecionado
                           : botaoFocoPadrao;
            if (alvo != null) SelecionarBotao(alvo);
            return; // navega no próximo frame, após restaurar foco
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
