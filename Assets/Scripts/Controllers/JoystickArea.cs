using UnityEngine;

public class JoysticArea : MonoBehaviour
{
    void Awake()
    {
        Canvas canvas = transform.GetComponentInParent<Canvas>();
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(canvas.transform.GetComponent<RectTransform>().sizeDelta.x, canvas.transform.GetComponent<RectTransform>().sizeDelta.y / 2);
    }
}
