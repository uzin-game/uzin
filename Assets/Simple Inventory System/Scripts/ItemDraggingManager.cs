using UnityEngine;

namespace RedstoneinventeGameStudio
{
    public class ItemDraggingManager : MonoBehaviour
    {
        public static CardManager dragCard;
        public static CardManager fromCard;
        public static CardManager toCard;

        [SerializeField] Vector3 tooltipOffset;
        [SerializeField] Vector3 draggingCardOffset;

        private void Update()
        {
            // Start dragging
            if (Input.GetKeyDown(KeyCode.Mouse0) && fromCard != null)
            {
                dragCard.SetItem(fromCard.itemData);
                fromCard.UnSetItem();
                dragCard.gameObject.SetActive(true);
            }

            // Dragging in progress
            if (dragCard != null && dragCard.gameObject.activeSelf)
            {
                dragCard.transform.position = Input.mousePosition + draggingCardOffset;
                TooltipManagerInventory.instance.transform.position = Input.mousePosition + tooltipOffset;
            }

            // Drop
            if (Input.GetKeyUp(KeyCode.Mouse0) && fromCard != null)
            {
                if (toCard != null)
                {
                    toCard.SetItem(dragCard.itemData);
                }
                else
                {
                    fromCard.SetItem(dragCard.itemData); // Revert drag
                }

                dragCard.gameObject.SetActive(false);
                fromCard = null;
                toCard = null;
            }
        }
    }
}