using UnityEngine;

/// <summary>
/// Detecta automaticamente qual joystick físico é P1 e qual é P2.
/// Coloque este script num GameObject da cena MenuPrincipal junto com
/// o GerenciadorDeAudio (ou em qualquer objeto com DontDestroyOnLoad).
/// 
/// Regra simples: o primeiro controle que apertar qualquer botão vira P1.
/// O segundo vira P2. Isso resolve o problema de ordem de plugagem.
/// 
/// Os outros scripts NÃO precisam mudar — eles continuam usando
/// "joystick 1" e "joystick 2". Este script apenas reordena os
/// joysticks físicos para que o certo seja sempre o 1 e o 2.
/// 
/// ATENÇÃO: como o Unity legado não permite remapear joysticks em runtime,
/// a abordagem aqui é diferente: guardamos qual número de joystick
/// pertence a cada player e os outros scripts consultam esta classe.
/// </summary>
public class GerenciadorControles : MonoBehaviour
{
    public static GerenciadorControles Instancia { get; private set; }

    // Número do joystick Unity (1 ou 2) atribuído a cada player
    // Por padrão: P1 = joystick 1, P2 = joystick 2
    public static int JoystickP1 { get; private set; } = 1;
    public static int JoystickP2 { get; private set; } = 2;

    // Retorna o prefixo correto para GetKeyDown, ex: "joystick 1 button 1"
    public static string BotaoP1(int numeroDoButton) => $"joystick {JoystickP1} button {numeroDoButton}";
    public static string BotaoP2(int numeroDoButton) => $"joystick {JoystickP2} button {numeroDoButton}";

    private bool deteccaoConcluida = false;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (deteccaoConcluida) return;
        DetectarOrdemJoysticks();
    }

    void DetectarOrdemJoysticks()
    {
        // Verifica quantos joysticks estão conectados
        string[] joysticks = Input.GetJoystickNames();
        int conectados = 0;
        for (int i = 0; i < joysticks.Length; i++)
            if (!string.IsNullOrEmpty(joysticks[i])) conectados++;

        if (conectados == 0) return;

        // Se só tem um controle plugado, ele é sempre P1
        if (conectados == 1)
        {
            // Descobre qual slot (1-4) tem o controle plugado
            for (int slot = 1; slot <= 4; slot++)
            {
                if (slot - 1 < joysticks.Length && !string.IsNullOrEmpty(joysticks[slot - 1]))
                {
                    JoystickP1 = slot;
                    // P2 recebe o próximo slot disponível (mesmo que vazio)
                    JoystickP2 = slot == 1 ? 2 : 1;
                    deteccaoConcluida = true;
                    Debug.Log($"[GerenciadorControles] 1 controle detectado. P1=joystick {JoystickP1}");
                    return;
                }
            }
        }

        // Com dois ou mais controles, usa a ordem padrão do Unity
        // (joystick 1 = P1, joystick 2 = P2)
        if (conectados >= 2)
        {
            JoystickP1 = 1;
            JoystickP2 = 2;
            deteccaoConcluida = true;
            Debug.Log($"[GerenciadorControles] 2 controles detectados. P1=joystick 1, P2=joystick 2");
        }
    }
}
