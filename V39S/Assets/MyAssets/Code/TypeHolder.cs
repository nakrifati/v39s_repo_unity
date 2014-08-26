using UnityEngine;

public class TypeHolder : MonoBehaviour
{
    public string type;

    void Start()
    {

    }

    public void AddMeToSave()
    {
        GameObject.FindGameObjectWithTag("SaveLoad").GetComponent<SaveAndLoad>().objects.Add(gameObject);
    }
}