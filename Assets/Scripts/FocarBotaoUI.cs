using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Precisamos disso para falar com o EventSystem

public class FocarBotaoUI : MonoBehaviour
{
    [Header("Ao Abrir o Painel")]
    [SerializeField] private Button botaoParaFocarAoAbrir;

    [Header("Ao Fechar o Painel (Opcional)")]
    [SerializeField] private Button botaoParaFocarAoFechar;

    void OnEnable()
    {
        if (botaoParaFocarAoAbrir != null)
        {
            // Limpa qualquer foco bugado e força o foco no botão de voltar
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botaoParaFocarAoAbrir.gameObject);
        }
    }

    public void VoltarParaOMenu()
    {
        if (botaoParaFocarAoFechar != null)
        {
            // Limpa e devolve o foco para o botão do menu principal à força
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botaoParaFocarAoFechar.gameObject);
        }
        
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Se a Unity perder o foco do nada (por causa do mouse), a gente força de volta!
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Se o painel está aberto, foca no Voltar. Se não, foca no botão do Menu.
            if (gameObject.activeInHierarchy && botaoParaFocarAoAbrir != null)
                EventSystem.current.SetSelectedGameObject(botaoParaFocarAoAbrir.gameObject);
            else if (botaoParaFocarAoFechar != null)
                EventSystem.current.SetSelectedGameObject(botaoParaFocarAoFechar.gameObject);
        }
    }
}