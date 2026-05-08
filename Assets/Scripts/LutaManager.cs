using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LutaManager : MonoBehaviour
{
    public Lutador p1;
    public Lutador p2;
    public TextMeshProUGUI textoVitoria;
    public GameObject painelFimDeJogo; // Opcional: um painel que aparece no fim

    void Start()
    {
        textoVitoria.text = "";
        if(painelFimDeJogo) painelFimDeJogo.SetActive(false);
    }

    public void VerificarVencedor(Lutador quemMorreu)
    {
        if(painelFimDeJogo) painelFimDeJogo.SetActive(true);

        if (quemMorreu == p1)
        {
            textoVitoria.text = p2.nomeLutador + " VENCEU!";
        }
        else
        {
            textoVitoria.text = p1.nomeLutador + " VENCEU!";
        }

        // Reinicia a luta depois de 5 segundos
        Invoke("ReiniciarCena", 5f);
    }

    void ReiniciarCena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}