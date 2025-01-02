using UnityEngine;
using UnityEngine.EventSystems;

public enum SwipeDirection { Up, Down }
public class HoverSwiper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SwipeDirection direction;
    public bool isHovered = false;
    public float timer = 0;

    public void FixedUpdate()
    {
        if (isHovered)
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                timer = 0;

                if (direction == SwipeDirection.Up)
                    InventoryManager.instance.NextPage(force: true);
                else if (direction == SwipeDirection.Down)
                    InventoryManager.instance.PrevPage(force: true);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InventoryManager.instance.isDragging) return;
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!InventoryManager.instance.isDragging) return;
        isHovered = false;
        timer = 0;
    }
}
