using UnityEngine;

public class GerenciadorDeAudio : MonoBehaviour
{
    [Header("Configuração da Música")]
    public AudioClip musicaDeFundo; // É aqui que você vai arrastar o seu áudio!
    
    [Range(0f, 1f)] // Cria uma barrinha charmosa no Inspector de 0 a 1
    public float volumeDaMusica = 0.5f; 

    private AudioSource tocador; // O "rádio" invisível que toca a música

    void Start()
    {
        // Se não tivermos arrastado nenhuma música, o script não faz nada para não dar erro
        if (musicaDeFundo == null) return;

        // 1. Cria o tocador de áudio no objeto automaticamente
        tocador = gameObject.AddComponent<AudioSource>();

        // 2. Coloca o seu áudio lá dentro
        tocador.clip = musicaDeFundo;

        // 3. Diz para a música repetir para sempre (Loop)
        tocador.loop = true;

        // 4. Ajusta o volume
        tocador.volume = volumeDaMusica;

        // 5. Dá o Play!
        tocador.Play();
    }
}