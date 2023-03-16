using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;


public class SpawnArmor : MonoBehaviour
{

    private Renderer rend;
    private GameObject current;
    private string currentAlienName;
    private bool state;
    private Toggle touchToggle;


    private void SelectAlien(string name)

    {
        if (name == GameManager.GetCurrent())
        {
            //Debug.Log("Return reached");
            if (GameObject.Find("Canvas").GetComponentsInChildren<Toggle>()[1].isOn == true)
            {
                OrbitCamera2.SetNewResetCamPos(true);
            }
            return;
        }
        else
        {
            Destroy(GameObject.Find(GameManager.GetCurrent() + "(Clone)"));
			current = Instantiate(Resources.Load(name)) as GameObject;
            //Debug.Log("Current model is " + name);
            GameManager.SetCurrent(name);
  //          OnTogglePushed(state);

 //           if (GameObject.Find("Canvas").GetComponentsInChildren<Toggle>()[1].isOn == true)
//            {

//                OrbitCamera2.SetNewResetCamPos(true);
//            }
        }
    }


/*    public void OnTogglePushed(bool WireOn)
    {

        if (WireOn)
        {
            //Debug.Log("pressed");
            Material newMat = Resources.Load("Materials/Wireframe", typeof(Material)) as Material;

            GameObject.Find(GameManager.GetCurrent() + "(Clone)").GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            GameObject.Find(GameManager.GetCurrent() + "(Clone)").GetComponentInChildren<Renderer>().sharedMaterial = newMat;
            state = true;
        }
        else
        {
            Material newMat = Resources.Load("Materials/" + GameManager.GetCurrent().Substring(0, 5) + Regex.Match(GameManager.GetCurrent(), @"\d+").Value + "Mat", typeof(Material)) as Material;

            GameObject.Find(GameManager.GetCurrent() + "(Clone)").GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            GameObject.Find(GameManager.GetCurrent() + "(Clone)").GetComponentInChildren<Renderer>().sharedMaterial = newMat;
            state = false;
        }
    } 
*/
}