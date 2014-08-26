using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SaveLoad"))
            Destroy(obj);
        Instantiate(Resources.Load("Objects/SaveLoad"));
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 35, 100, 30), "New Game"))
        {
            System.IO.File.Delete("Save.save");
            Application.LoadLevel("Main");
            Destroy(gameObject);
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 30), "Load Last"))
        {
            if (System.IO.File.Exists("Save.save"))
            {
                Application.LoadLevel("Main");
                GameObject g = new GameObject("ToLoad");
                DontDestroyOnLoad(g);
                //GameObject.FindGameObjectWithTag("SaveLoad").GetComponent<SaveAndLoad>().Load("Save.save");
                Destroy(gameObject);
            }
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 35, 100, 30), "Exit"))
        {
            Application.Quit();
        }
    }

    public void DeleteCamera()
    {
        Destroy(camera);
        Destroy(audio);
    }
}
