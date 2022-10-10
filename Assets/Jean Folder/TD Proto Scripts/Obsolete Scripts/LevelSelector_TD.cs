using UnityEngine;
using UnityEngine.UI;

public class LevelSelector_TD : MonoBehaviour
{
    public SceneFader fader;

    public Button[] levelbuttons;

    private void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);


        //for (int i = 0; i < levelbuttons.Length; i++)
        //{
        //    i


        //    if (i + 1 < levelReached)
        //    {
        //        levelbuttons[i].interactable = false;

        //    }
        //}
    }



    public void Select(string levelName)
        {

        fader.FadeTo(levelName);
        }


}
