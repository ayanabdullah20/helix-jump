using UnityEngine;
using System.Collections.Generic;

public class Stack : MonoBehaviour
{
    #region Unity Methods
    void Start()
    {
        // generateRandomStack();
    }
    #endregion
    public List<GameObject> StackElements = new List<GameObject>();
    public Material safeMaterial;
    public Material dangerMaterial;
    public void generateRandomStack(bool isfirststack = false)
    {
        if (isfirststack)
        {
            foreach (var element in StackElements)
            {
                element.GetComponent<Renderer>().material = safeMaterial;
                element.tag = "Safe";
            }
            StackElements[2].SetActive(false);
        }
        else
        {
            int totalstackstillnow = (int)GameManager.instance.score / 100;
            int difficultylevel = totalstackstillnow / 3;
            int totaldangerelementcount;
            if ((difficultylevel == 0) || (difficultylevel == 1))
            {
                totaldangerelementcount = Random.Range(1, 3);
            }
            else if (difficultylevel == 2)
            {
                totaldangerelementcount = Random.Range(1, 3);
            }
            else if (difficultylevel == 3)
            {
                totaldangerelementcount = Random.Range(2, 4);
            }
            else
            {
                totaldangerelementcount = Random.Range(2, 5);
            }
            int emptyelementindex = Random.Range(0, StackElements.Count);
            List<int> dangerelementindices = new List<int>();
            for (int i = 0; i < totaldangerelementcount; i++)
            {
                int foundindex;
                do
                {
                    foundindex = Random.Range(0, StackElements.Count);

                }
                while (foundindex == emptyelementindex || dangerelementindices.Contains(foundindex));
                dangerelementindices.Add(foundindex);
            }
            foreach (var element in StackElements)
            {
                element.GetComponent<Renderer>().material = safeMaterial;
                element.tag = "Safe";
            }


            StackElements[emptyelementindex].SetActive(false);
            foreach (var index in dangerelementindices)
            {
                StackElements[index].GetComponent<Renderer>().material = dangerMaterial;
                StackElements[index].tag = "Danger";
            }

        }

    }
}
