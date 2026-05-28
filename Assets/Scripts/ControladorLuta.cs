using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; 

public class ControladorLuta : MonoBehaviour
{
    [Header("Configurações da Partida")]
    public float tempoDoRound = 99f;
    public int roundsParaVencer = 2;
    
    private float tempoAtual;
    private bool lutaAtiva = false;
    private int roundAtual = 1;
    private int vitoriasP1 = 0;
    private int vitoriasP2 = 0;

    [Header("Configurações de Áudio do Juiz")]
    public AudioSource audioSource;
    public AudioClip somRound;
    public AudioClip[] sonsNumeros; 
    public AudioClip somFight;
    [Tooltip("Som do juiz falando YOU WIN no final da partida")]
    public AudioClip somYouWin; 

    [Header("Componentes de UI (Vitoria)")]
    public GameObject painelVitoria; 
    [Tooltip("Arraste os ícones apagados (ex: estrelas/v) do P1 aqui")]
    public GameObject[] iconesVitoriaP1; 
    [Tooltip("Arraste os ícones apagados (ex: estrelas/v) do P2 aqui")]
    public GameObject[] iconesVitoriaP2;

    [Header("Componentes de UI (Automáticos)")]
    private TextMeshProUGUI txtTempo;
    private TextMeshProUGUI txtCentro;
    private TextMeshProUGUI txtVencedor;

    [Header("Lutadores na Cena")]
    private Lutador player1;
    private Lutador player2;

    void Start()
    {
        tempoAtual = tempoDoRound;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        GameObject objTempo = GameObject.Find("Texto_Tempo");
        if (objTempo != null) txtTempo = objTempo.GetComponent<TextMeshProUGUI>();

        GameObject objCentro = GameObject.Find("Texto_Centro");
        if (objCentro != null) txtCentro = objCentro.GetComponent<TextMeshProUGUI>();

        if (painelVitoria != null)
        {
            txtVencedor = painelVitoria.GetComponentInChildren<TextMeshProUGUI>();
            painelVitoria.SetActive(false); 
        }

        // Garante que os ícones comecem desligados
        DesligarIconesVitoria();

        Lutador[] lutadores = FindObjectsOfType<Lutador>();
        foreach (Lutador l in lutadores)
        {
            if (l.isPlayer1) player1 = l;
            else player2 = l;
        }

        if (player1 != null) player1.ConfigurarUIInicial();
        if (player2 != null) player2.ConfigurarUIInicial();

        StartCoroutine(SequenciaInicial());
    }

    void DesligarIconesVitoria()
    {
        foreach(GameObject icone in iconesVitoriaP1) if(icone != null) icone.SetActive(false);
        foreach(GameObject icone in iconesVitoriaP2) if(icone != null) icone.SetActive(false);
    }

    void Update()
    {
        if (!lutaAtiva) return;

        if (tempoAtual > 0)
        {
            tempoAtual -= Time.deltaTime;
            if (txtTempo != null) txtTempo.text = Mathf.CeilToInt(tempoAtual).ToString();
        }
        else
        {
            tempoAtual = 0;
            if (txtTempo != null) txtTempo.text = "0";
            AcabouOTempo();
        }
    }

    IEnumerator SequenciaInicial()
    {
        ConfigurarScriptsLutadores(false);
        yield return null; 

        if (txtCentro != null)
        {
            txtCentro.gameObject.SetActive(true);
            txtCentro.text = "ROUND " + roundAtual;
            
            if (audioSource != null)
            {
                if (somRound != null) audioSource.PlayOneShot(somRound);
                yield return new WaitForSeconds(0.6f); 

                int indexNumero = roundAtual - 1;
                if (sonsNumeros != null && indexNumero >= 0 && indexNumero < sonsNumeros.Length)
                {
                    if (sonsNumeros[indexNumero] != null) audioSource.PlayOneShot(sonsNumeros[indexNumero]);
                }
            }

            yield return new WaitForSeconds(1.2f);

            txtCentro.text = "FIGHT!";
            if (audioSource != null && somFight != null)
            {
                audioSource.PlayOneShot(somFight);
            }

            yield return new WaitForSeconds(1f);
            txtCentro.gameObject.SetActive(false);
        }

        ConfigurarScriptsLutadores(true);
        lutaAtiva = true;
    }

    public void FinalizarRoundPorNocaute(Lutador perdedor)
    {
        if (!lutaAtiva) return;
        lutaAtiva = false;

        Lutador vencedor = (perdedor == player1) ? player2 : player1;

        if (txtCentro != null)
        {
            txtCentro.gameObject.SetActive(true);
            txtCentro.text = "";
        }

        if (vencedor != null) vencedor.ComemorarVitoria();

        StartCoroutine(GerenciarFimDeRound(vencedor));
    }

    private void AcabouOTempo()
    {
        if (!lutaAtiva) return;
        lutaAtiva = false;

        ConfigurarScriptsLutadores(false);

        if (txtCentro != null)
        {
            txtCentro.gameObject.SetActive(true);
            txtCentro.text = "TIME UP";
        }

        Lutador vencedor = null;
        if (player1.currentLife > player2.currentLife) vencedor = player1;
        else if (player2.currentLife > player1.currentLife) vencedor = player2;

        if (vencedor != null) vencedor.ComemorarVitoria();

        StartCoroutine(GerenciarFimDeRound(vencedor));
    }

    IEnumerator GerenciarFimDeRound(Lutador vencedor)
    {
        // Define o placar e liga o ícone correspondente na tela
        if (vencedor == player1) 
        {
            vitoriasP1++;
            if (vitoriasP1 - 1 < iconesVitoriaP1.Length && iconesVitoriaP1[vitoriasP1 - 1] != null)
            {
                iconesVitoriaP1[vitoriasP1 - 1].SetActive(true);
            }
        }
        else if (vencedor == player2) 
        {
            vitoriasP2++;
            if (vitoriasP2 - 1 < iconesVitoriaP2.Length && iconesVitoriaP2[vitoriasP2 - 1] != null)
            {
                iconesVitoriaP2[vitoriasP2 - 1].SetActive(true);
            }
        }

        yield return new WaitForSeconds(3f);

        if (txtCentro != null) txtCentro.gameObject.SetActive(false);

        // Verifica se alguém ganhou a partida definitiva
        if (vitoriasP1 >= roundsParaVencer || vitoriasP2 >= roundsParaVencer)
        {
            // Toca a voz de YOU WIN
            if (audioSource != null && somYouWin != null)
            {
                audioSource.PlayOneShot(somYouWin);
            }
            
            // Dá um tempinho para a voz soar antes de abrir o painel
            yield return new WaitForSeconds(1.5f);

            if (painelVitoria != null)
            {
                painelVitoria.SetActive(true); 
                if (txtVencedor != null)
                {
                    if (vitoriasP1 >= roundsParaVencer) txtVencedor.text = player1.nomeLutador.ToUpper() + " WIN!";
                    else txtVencedor.text = player2.nomeLutador.ToUpper() + " WIN!";
                }

                Button primeiroBotao = painelVitoria.GetComponentInChildren<Button>();
                if (primeiroBotao != null && EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null); 
                    EventSystem.current.SetSelectedGameObject(primeiroBotao.gameObject); 
                }
            }
        }
        else
        {
            roundAtual++;
            
            if (player1 != null) player1.ResetarParaNovoRound();
            if (player2 != null) player2.ResetarParaNovoRound();
            
            tempoAtual = tempoDoRound;

            StartCoroutine(SequenciaInicial());
        }
    }

    private void ConfigurarScriptsLutadores(bool ativo)
    {
        if (player1 != null) player1.enabled = ativo;
        if (player2 != null) player2.enabled = ativo;
    }

    public void ReiniciarPartida()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VoltarSelecao()
    {
        SceneManager.LoadScene("SelecaoDePersonagem"); 
    }

    public void SairParaMenu()
    {
        SceneManager.LoadScene("MenuPrincipal"); 
    }
}