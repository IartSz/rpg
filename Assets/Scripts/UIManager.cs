using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    [Header("Textos (arrastrar desde la jerarquía)")]
    public TextMeshProUGUI textoOro;
    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoCostoProximo;
    public TextMeshProUGUI textoMonedaPrestige;
    public TextMeshProUGUI textoBonoPrestige;

    [Header("Botones (arrastrar desde la jerarquía)")]
    public Button botonMejorar;
    public Button botonPrestige;

    [Header("Refresco de UI")]
    [Tooltip("Cada cuántos segundos se actualiza el texto. No hace falta que sea cada frame.")]
    public float intervaloRefrescoSegundos = 0.2f;

    private float acumuladorRefresco = 0f;

    void Start()
    {
        // Conectamos los botones a los metodos del GameManager.
        if (botonMejorar != null)
            botonMejorar.onClick.AddListener(OnClickMejorar);

        if (botonPrestige != null)
            botonPrestige.onClick.AddListener(OnClickPrestige);

        RefrescarUI();
    }

    void Update()
    {
        acumuladorRefresco += Time.deltaTime;
        if (acumuladorRefresco >= intervaloRefrescoSegundos)
        {
            acumuladorRefresco = 0f;
            RefrescarUI();
        }
    }

    private void OnClickMejorar()
    {
        bool pudoMejorar = GameManager.Instance.IntentarMejorarNivel();
        if (!pudoMejorar)
        {

            Debug.Log("No hay oro suficiente para mejorar.");
        }
        RefrescarUI();
    }

    private void OnClickPrestige()
    {

        GameManager.Instance.EjecutarPrestige();
        RefrescarUI();
    }

    private void RefrescarUI()
    {
        if (GameManager.Instance == null) return;

        if (textoOro != null)
            textoOro.text = $"Oro: {GameManager.Instance.Oro:N0}";

        if (textoNivel != null)
            textoNivel.text = $"Nivel: {GameManager.Instance.Nivel}";

        if (textoCostoProximo != null)
            textoCostoProximo.text = $"Costo mejora: {GameManager.Instance.ProximoCosto:N0}";

        if (textoMonedaPrestige != null)
            textoMonedaPrestige.text = $"Prestige: {GameManager.Instance.MonedaPrestige}";

        if (textoBonoPrestige != null)
            textoBonoPrestige.text = $"Bono: x{GameManager.Instance.ObtenerBonoPermanentePrestige():N2}";


        if (botonMejorar != null)
            botonMejorar.interactable = GameManager.Instance.Oro >= GameManager.Instance.ProximoCosto;
    }
}
