using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Método para o botão "INICIAR" chamar
    public void IniciarJogo()
    {
        Debug.Log("Indo para a Seleção de Personagens!");
        SceneManager.LoadScene("SelecaoDePersonagem"); // Nome corrigido e exato
    }

    // Método para o botão "CREDITOS" chamar
    public void AbrirCreditos()
    {
        Debug.Log("Tela de Créditos (Ainda vamos fazer!)");
        // Quando criar o painel de créditos, a lógica de ligar ele virá aqui
    }

    // Método extra útil: Fechar o jogo (caso queira colocar um botão "Sair")
    public void SairDoJogo()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit(); // Fecha o jogo quando estiver compilado (.exe)
    }
}