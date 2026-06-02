using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorDeAudio : MonoBehaviour
{
    [Header("Músicas")]
    public AudioClip musicaMenu;
    public AudioClip musicaLuta;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource tocador;

    // Singleton — garante que só existe um na memória
    private static GerenciadorDeAudio instancia;

    void Awake()
    {
        // Se já existe um GerenciadorDeAudio rodando, este se destrói
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;

        // Não destrói ao trocar de cena
        DontDestroyOnLoad(gameObject);

        tocador = gameObject.AddComponent<AudioSource>();
        tocador.loop = true;
        tocador.volume = volume;

        // Começa tocando a música do menu
        TocaMusica(musicaMenu);

        // Registra para ser avisado sempre que uma cena nova carregar
        SceneManager.sceneLoaded += OnCenaCarregada;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnCenaCarregada;
    }

    void OnCenaCarregada(Scene cena, LoadSceneMode mode)
    {
        switch (cena.name)
        {
            case "MenuPrincipal":
            case "SelecaoDePersonagem":
            case "SelecaoDeMapas":
                TocaMusica(musicaMenu);
                break;

            case "CenaDeLuta":
                TocaMusica(musicaLuta);
                break;

            // LoadingVersus e outras cenas: não mexe na música (silêncio ou mantém)
            default:
                break;
        }
    }

    void TocaMusica(AudioClip clip)
    {
        if (clip == null) return;

        // Se já está tocando essa mesma música, não reinicia
        if (tocador.clip == clip && tocador.isPlaying) return;

        tocador.clip = clip;
        tocador.Play();
    }
}
