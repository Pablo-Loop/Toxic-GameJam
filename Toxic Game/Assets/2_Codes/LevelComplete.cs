using UnityEngine;
using UnityEngine.SceneManagement; 

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private string siguienteEscena;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (PlayerPrefs.GetInt("level") < level)
            {
                PlayerPrefs.SetInt("level", level);
                PlayerPrefs.Save();
            }

            Debug.Log("Paso el nivel: " +  level);

            SceneManager.LoadScene(siguienteEscena);
        }
    }

}
