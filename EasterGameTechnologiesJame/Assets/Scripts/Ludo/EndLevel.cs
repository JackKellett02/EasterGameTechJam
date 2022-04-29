using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<PlayerPlatformer>().PlayerDeathToggleOn(Vector2.zero);
            StartCoroutine("LevelSelect");
        }
    }

    IEnumerator LevelSelect()
    {
        GameObject.FindGameObjectWithTag("CircleWipe").GetComponent<CircleWipe>().WipeIn(1);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(0);
    }
}
