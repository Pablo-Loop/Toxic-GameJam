using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject LevelSelectorCanva;

    // Iniciar juego
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    // Abrir ventana controles
    public void LevelSelector()
    {
        LevelSelectorCanva.SetActive(true);
    }

    // Cerrar ventana controles
    public void CloseLevelSelector()
    {
        LevelSelectorCanva.SetActive(false);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Salir del juego
    public void CloseGame()
    {
        SceneManager.LoadScene("Level 4");
        Application.Quit();
    }
}
