using UnityEngine;


public class UiLore : MonoBehaviour
{
    [SerializeField] private GameObject Lore1;
    [SerializeField] private GameObject Lore2;
    [SerializeField] private GameObject HUD;

    public void NextLore()
    {
        Lore1.SetActive(false);
        Lore2.SetActive(true);
    }

    public void LastLore()
    {
        Lore2.SetActive(false);

        // En lugar de HUD.SetActive(true), llamamos al UIManager:
        if (PiroBros.Managers.UIManager.Instance != null)
        {
            PiroBros.Managers.UIManager.Instance.ShowCharacterSelect();
        }
    }
}
