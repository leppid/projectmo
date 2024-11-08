using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoysticArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = this.transform.GetComponentInParent<Canvas>();
        this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(canvas.transform.GetComponent<RectTransform>().sizeDelta.x, canvas.transform.GetComponent<RectTransform>().sizeDelta.y / 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
