using UnityEngine;
using System.Collections;

public class Cheats : MonoBehaviour
{
    public bool consoleVisible, debugVisible, audio;
    public string stringToEdit;

    PlayerInfo pi;
    VaultEnvironment ve;

    public GameObject gui;

    void Start()
    {
        pi = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        ve = GameObject.FindGameObjectWithTag("VaultEnvironment").GetComponent<VaultEnvironment>();

		if (audio)
			GameObject.Find("MainCamera").GetComponent<AudioSource>().volume = 1;
		else GameObject.Find("MainCamera").GetComponent<AudioSource>().volume = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            consoleVisible = !consoleVisible;
    }

    void OnGUI()
    {
        if (consoleVisible)
        {
            stringToEdit = GUI.TextField(new Rect(10, 10, 200, 20), stringToEdit, 25);

            if (stringToEdit == "")//"BAGUVIX")
            {
                debugVisible = true;
                consoleVisible = false;
            }
            else
                debugVisible = false;
        }

        if (debugVisible)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                consoleVisible = false;
                debugVisible = false;
            }
            if (GUI.Button(new Rect(220, 70, 150, 30), "CloseDebug"))
            {
                consoleVisible = false;
                debugVisible = false;
            }
            if (GUI.Button(new Rect(10, 70, 150, 30), "Room Damage"))
            {
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>().GetMyRoom() != null)
                    GameObject.Find("Vault").GetComponent<VaultInfo>().GiveDamage(15, GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>().GetMyRoom());
            }
            if (GUI.Button(new Rect(10, 120, 150, 30), "Do Eartquake"))
            {
                ve.EarthqTimer = Time.timeSinceLevelLoad - 5;
            }
            if (GUI.Button(new Rect(10, 170, 150, 30), "Draw Gizmo"))
            {
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RadZone"))
                {
                    obj.renderer.enabled = !obj.renderer.enabled;
                }

                GameObject trg = GameObject.Find("Target(Clone)");
                if (trg)
                    trg.renderer.enabled = !trg.renderer.enabled;


                GameObject rad = GameObject.Find("Player Rad");
                if (rad)
                    rad.renderer.enabled = !rad.renderer.enabled;

                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Respawn"))
                {
                    obj.renderer.enabled = !obj.renderer.enabled;
                }
            }
            if (GUI.Button(new Rect(10, 220, 150, 30), "Toogle GUI"))
            {
                pi.pg.enabled = !pi.pg.enabled;
                gui.SetActive(!gui.activeSelf);
            }
            if (GUI.Button(new Rect(10, 270, 150, 30), "Toogle Audio"))
            {
                audio = !audio;
                if (audio)
                    GameObject.Find("MainCamera").GetComponent<AudioSource>().volume = 1;
                else GameObject.Find("MainCamera").GetComponent<AudioSource>().volume = 0;
            }


            if (GUI.Button(new Rect(10, 320, 150, 30), "AirCompDamage ||" + GameObject.Find("CompAir").GetComponent<Computers>().cd.health.ToString()))
            {
                GameObject.Find("CompAir").GetComponent<Computers>().GiveRegularDamage();
            }

            if (GUI.Button(new Rect(10, 370, 150, 30), "DoorCompDamage ||" + GameObject.Find("CompDoor").GetComponent<Computers>().cd.health.ToString()))
            {
                GameObject.Find("CompDoor").GetComponent<Computers>().GiveRegularDamage();
            }

            if (GUI.Button(new Rect(10, 420, 150, 30), "CompGreenhouse ||" + GameObject.Find("CompGreenhouse").GetComponent<Computers>().cd.health.ToString()))
            {
                GameObject.Find("CompGreenhouse").GetComponent<Computers>().GiveRegularDamage();
            }

            if (GUI.Button(new Rect(10, 470, 150, 30), "Kick Player"))
            {
                GameObject.Find("Player").GetComponent<PlayerInfo>().Health -= 10;
            }
        }
    }
}