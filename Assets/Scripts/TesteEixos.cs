using UnityEngine;

public class TesteEixos : MonoBehaviour
{
    void Update()
    {
        // Mostra nomes dos joysticks detectados pelo Unity
        string[] joysticks = Input.GetJoystickNames();
        string nomesLog = "JOYSTICKS: ";
        for (int i = 0; i < joysticks.Length; i++)
            nomesLog += $"[{i+1}]={( string.IsNullOrEmpty(joysticks[i]) ? "vazio" : joysticks[i])}  ";

        // Mostra eixos de ambos os controles
        string eixosLog =
            $"LX_P1:{Input.GetAxisRaw("PS4_LX_P1"):F2}  " +
            $"LY_P1:{Input.GetAxisRaw("PS4_LY_P1"):F2}  " +
            $"DpadX_P1:{Input.GetAxisRaw("PS4_DpadX_P1"):F2}  " +
            $"DpadY_P1:{Input.GetAxisRaw("PS4_DpadY_P1"):F2}  ||  " +
            $"LX_P2:{Input.GetAxisRaw("PS4_LX_P2"):F2}  " +
            $"LY_P2:{Input.GetAxisRaw("PS4_LY_P2"):F2}  " +
            $"DpadX_P2:{Input.GetAxisRaw("PS4_DpadX_P2"):F2}  " +
            $"DpadY_P2:{Input.GetAxisRaw("PS4_DpadY_P2"):F2}";

        // Mostra botões pressionados em qualquer joystick (0 a 3)
        string botoesLog = "BOTOES: ";
        for (int joy = 1; joy <= 2; joy++)
            for (int btn = 0; btn <= 9; btn++)
                if (Input.GetKeyDown($"joystick {joy} button {btn}"))
                    botoesLog += $"J{joy}B{btn} ";

        Debug.Log(nomesLog);
        Debug.Log(eixosLog);
        if (botoesLog != "BOTOES: ") Debug.Log(botoesLog);
    }
}
