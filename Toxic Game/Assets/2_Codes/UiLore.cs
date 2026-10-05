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
        HUD.SetActive(true);
    }
}
