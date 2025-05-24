using UnityEngine;

public class selectRecipe : MonoBehaviour
{
    public CraftingRecipes recipe;
    public GameObject ConstructorUsing;
    public void Onclick()
    {
        ConstructorUsing.GetComponent<ConstructeurUsing>().Recipe = recipe;
    }
}
