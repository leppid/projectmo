using System.Collections;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public GameObject Enter;
    public GameObject Vasya;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Enter.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Enter.SetActive(false);
        }
    }
    
    public void ShowVasya()
    {
        StartCoroutine(ShowVasyaEnum());
    }

    IEnumerator ShowVasyaEnum()
    {
        Enter.SetActive(false);
        Vasya.SetActive(true);

        yield return new WaitForSeconds(5f);

        Vasya.SetActive(false);
    }
}
