using UnityEngine;
using System.Collections.Generic;

public class Stack : MonoBehaviour
{
    #region Unity Methods
    void Start()
    {
        generateRandomStack();
    }
    #endregion
    public List<GameObject> StackElements = new List<GameObject>();
    public Material safeMaterial;
    public Material dangerMaterial;
    public void generateRandomStack()
    {
         int emptyelementindex = Random.Range(0, StackElements.Count);
         int dangerelementindex1 = Random.Range(0, StackElements.Count);
         while (dangerelementindex1 == emptyelementindex)
         {
             dangerelementindex1 = Random.Range(0, StackElements.Count);
         }
         int dangerelementindex2 = Random.Range(0, StackElements.Count);
         while (dangerelementindex2 == emptyelementindex || dangerelementindex2 == dangerelementindex1)
         {
             dangerelementindex2 = Random.Range(0, StackElements.Count);
         }   
         foreach (var element in StackElements)
         {
             element.GetComponent<Renderer>().material = safeMaterial;
             element.tag = "Safe";
         }
            StackElements[emptyelementindex].SetActive(false);
            StackElements[dangerelementindex1].GetComponent<Renderer>().material = dangerMaterial;
            StackElements[dangerelementindex1].tag = "Danger";
            StackElements[dangerelementindex2].tag = "Danger";
            StackElements[dangerelementindex2].GetComponent<Renderer>().material = dangerMaterial;

    }
}
