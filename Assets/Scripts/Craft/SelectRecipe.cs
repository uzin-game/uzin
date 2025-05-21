using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectRecipe : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI NameRecipe;
    public TextMeshProUGUI QuantityRecipe;
    public CraftingRecipes recipe;
    
    public Image Selectedimage;
    public TextMeshProUGUI SelectedNameRecipe;
    public TextMeshProUGUI SelectedQuantityRecipe;
    public GameObject SelectButton;

    public void OnPressed()
    {
        Selectedimage.sprite = image.sprite;
        SelectedNameRecipe.text = NameRecipe.text;
        SelectedQuantityRecipe.text = QuantityRecipe.text;
        SelectButton.GetComponent<Craft>().re = recipe;
    }
}
