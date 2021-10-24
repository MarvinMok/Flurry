using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public GameObject mainMenu;
	public Scene gameScene;

    // Start is called before the first frame update
    void Start()
    {

        toMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
	{
		SceneManager.LoadScene("Tyler'sScene");
	}

	public void quit()
	{
		Application.Quit();
	}

	public void toMainMenu()
	{
		mainMenu.SetActive(true);
	}
}
