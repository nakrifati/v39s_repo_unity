using UnityEngine;
using System.Collections.Generic;

public class UseLight : MonoBehaviour
{
    public Color32 startColor, newColor, currColor;
    public float lerpSpeed;
    public bool useMonoMouseHover;

    void Start()
    {
        currColor = startColor;
    }

    void Update()
    {
        if (renderer) // Рекурсия? Не, не слышал
        {
            renderer.material.color = Color.Lerp(renderer.material.color, currColor, lerpSpeed * Time.deltaTime);
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            GetComponent<SkinnedMeshRenderer>().material.color = Color.Lerp(renderer.material.color, currColor, lerpSpeed * Time.deltaTime);
        }
        else for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<Renderer>())
                    transform.GetChild(i).renderer.material.color = Color.Lerp(transform.GetChild(i).renderer.material.color, currColor, lerpSpeed * Time.deltaTime);

                if (transform.GetChild(i).GetComponent<SkinnedMeshRenderer>())
                    transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = Color.Lerp(transform.GetChild(i).renderer.material.color, currColor, lerpSpeed * Time.deltaTime);

                for (int j = 0; j < transform.GetChild(i).childCount; j++)
                {
                    if (transform.GetChild(i).transform.GetChild(j).GetComponent<Renderer>())
                        transform.GetChild(i).transform.GetChild(j).GetComponent<Renderer>().material.color = Color.Lerp(transform.GetChild(i).transform.GetChild(j).renderer.material.color, currColor, lerpSpeed * Time.deltaTime);

                    if (transform.GetChild(i).transform.GetChild(j).GetComponent<SkinnedMeshRenderer>())
                        transform.GetChild(i).transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material.color = Color.Lerp(transform.GetChild(i).transform.GetChild(j).renderer.material.color, currColor, lerpSpeed * Time.deltaTime);
                }

            }
    }

    public void SetNewMat()
    {
        currColor = newColor;
    }

    public void SetStartMat()
    {
        currColor = startColor;
    }

    void OnMouseOver()
    {
        if (useMonoMouseHover)
            SetNewMat();
    }

    void OnMouseExit()
    {
        if (useMonoMouseHover)
            SetStartMat();
    }
}
