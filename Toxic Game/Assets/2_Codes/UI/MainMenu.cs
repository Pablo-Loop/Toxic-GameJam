using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelControles;

    // Iniciar juego
    public void IniciarJuego()
    {
        SceneManager.LoadScene("Nivel1");
    }

    // Abrir ventana controles
    public void AbrirControles()
    {
        panelControles.SetActive(true);
    }

    // Cerrar ventana controles
    public void CerrarControles()
    {
        panelControles.SetActive(false);
    }

    // Salir del juego
    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
