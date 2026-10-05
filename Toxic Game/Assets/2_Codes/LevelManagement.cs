using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManagement : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private Button btn;
    [SerializeField] private TMP_Text txt;

    [Header("Configuración de Carga")]
    [SerializeField] private string levelScenePrefix = "Level ";

    public void Start()
    {
        //PlayerPrefs.DeleteKey("level");
        //PlayerPrefs.Save();

        if (txt != null)
        {
            txt.text = level.ToString();
        }

        int levelFinished = PlayerPrefs.GetInt("level", 1);

        if (btn != null)
        {
            btn.interactable = (level <= levelFinished);
        }
    }

    public void OpenLevel()
    {
        string sceneToLoad = levelScenePrefix;
        Debug.Log("Cargando escena: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}