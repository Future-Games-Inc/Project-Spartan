using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is made to be used for the Demo only. It is not part of this assets pack.

public class UIManager : MonoBehaviour
{
    [Header ("UI Control Panel")]
    public GameObject panelTop;
    public GameObject panelMiddleTop;
    public GameObject panelBottom;
    public GameObject panelControlMain;
    public GameObject panelControlEffectMode;
    public GameObject panelControlSCBlue;
    public GameObject panelControlSCCyan;
    public GameObject panelControlSCLime;
    public GameObject panelControlSCPurple;
    public GameObject[] buttonColor;
    public Color uiPanelColor;
    public Color uiButtonColor;

    [Header ("UI Text")]
    public Text primaryColor;
    public Text effectMode;
    public Text secondaryColor;
    public Text instructionText;
    public static string primaryColorStr;
    public static string effectModeStr;
    public static string secondaryColorStr;

    [Header ("Weapons Room")]
    public GameObject weaponsRoom;
    public GameObject[] displays;
    public Color32 colorRoom;
    public static Color32 colorDisplay;

    string[] colorStr = new string[5]{"Red", "Blue", "Cyan", "Lime", "Purple"};
    string[] effectStr = new string[3] {"Single", "Mod", "Fill"};
    string[] instructionStr = new string[3] 
    {
        "Choose a color, or click on a weapon for preview.",
        "Choose the effect mode.",
        "Choose the secondary color for the effect."
    };

    // Start is called before the first frame update
    void Start()
    {
        colorDisplay = new Color32(255,0,0,255);

        primaryColorStr = primaryColor.text;
        effectModeStr = effectMode.text;
        secondaryColorStr = secondaryColor.text;

        buttonColor = GameObject.FindGameObjectsWithTag("ButtonColor");
    }

    // Update is called once per frame
    void Update()
    {
        primaryColorStr = primaryColor.text;
        effectModeStr = effectMode.text;
        secondaryColorStr = secondaryColor.text;

        buttonColor = GameObject.FindGameObjectsWithTag("ButtonColor");

        if(primaryColor.text == colorStr[0])
        {
            UIColor(195,0,0,255,195,0,0,255);
        }
        else if(primaryColor.text == colorStr[1])
        {
            UIColor(214,0,255,255,12,95,232,255);
        }
        else if(primaryColor.text == colorStr[2])
        {
            UIColor(214,0,255,255,0,184,255,255);
        }
        else if(primaryColor.text == colorStr[3])
        {
            UIColor(214,0,255,255,0,255,159,255);
        }
        else if(primaryColor.text == colorStr[4])
        {
            UIColor(0,184,255,255,214,0,255,255);
        }
    }

    //Primary Color
    //Main Control Panel
    #region Main Control Panel
        public void PCM_ButtonRed()
        {
            primaryColor.text = colorStr[0];
            effectMode.text = effectStr[0];
            secondaryColor.text = "None";
            instructionText.text = instructionStr[0];
            WeaponRoomDisplayColor(255,0,0,255,255);
        }

        public void PCM_ButtonBlue()
        {
            primaryColor.text = colorStr[1];
            effectMode.text = effectStr[0];
            secondaryColor.text = "None";
            instructionText.text = instructionStr[0];
            WeaponRoomDisplayColor(12,95,232,0,255);
        }

        public void PCM_ButtonCyan()
        {
            primaryColor.text = colorStr[2];
            effectMode.text = effectStr[0];
            secondaryColor.text = "None";
            instructionText.text = instructionStr[0];
            WeaponRoomDisplayColor(0,184,255,0,255);
        }

        public void PCM_ButtonLime()
        {
            primaryColor.text = colorStr[3];
            effectMode.text = effectStr[0];
            secondaryColor.text = "None";
            instructionText.text = instructionStr[0];
            WeaponRoomDisplayColor(0,255,159,0,255);
        }

        public void PCM_ButtonPurple()
        {
            primaryColor.text = colorStr[4];
            effectMode.text = effectStr[0];
            secondaryColor.text = "None";
            instructionText.text = instructionStr[0];
            WeaponRoomDisplayColor(214,0,255,0,255);
        }

        public void PCM_ButtonEffectMode()
        {
            if(primaryColor.text != colorStr[0])
            {
                panelControlMain.SetActive(false);
                panelControlEffectMode.SetActive(true);
                instructionText.text = instructionStr[1];
            }
        }

        public void PCM_ButtonReset()
        {
            primaryColor.text = colorStr[0];
            effectMode.text = effectStr[0];
            secondaryColor.text = "None";
            instructionText.text = instructionStr[0];
            WeaponRoomDisplayColor(255,0,0,255,255);
        }
    #endregion
    //End of Main Control Panel

    //Effect Mode
    //Effect Mode Control Panel
    #region Effect Mode Control Panel  
        public void PCEM_ButtonMod()
        {
            effectMode.text = effectStr[1];
            instructionText.text = instructionStr[2];
            panelControlEffectMode.SetActive(false);
            PrimColorToSecColor();
        }

        public void PCEM_ButtonFill()
        {
            effectMode.text = effectStr[2];
            instructionText.text = instructionStr[2];
            panelControlEffectMode.SetActive(false);
            PrimColorToSecColor();
        }

        public void PCEM_ButtonBack()
        {
            instructionText.text = instructionStr[0];
            panelControlMain.SetActive(true);
            panelControlEffectMode.SetActive(false);
        }

        public void PCEM_ButtonReset()
        {
            primaryColor.text = colorStr[0];
            effectMode.text = effectStr[0];
            secondaryColor.text = "None";
            instructionText.text = instructionStr[0];
            panelControlMain.SetActive(true);
            panelControlEffectMode.SetActive(false);
            WeaponRoomDisplayColor(255,0,0,255,255);
        }
    #endregion
    //End of Effect Mode Control Panel

    //Secondary Color
    //Secondary Color Control Panel
    #region Secondary Color Control Panel
        //Secondary Color Blue Control Panel
        #region Secondary Color Blue Control Panel
            public void PCSCB_ButtonCyan()
            {
                secondaryColor.text = colorStr[2];
            }

            public void PCSCB_ButtonLime()
            {
                secondaryColor.text = colorStr[3];
            }

            public void PCSCB_ButtonPurple()
            {
                secondaryColor.text = colorStr[4];
            }

            public void PCSCB_ButtonBack()
            {
                instructionText.text = instructionStr[1];
                panelControlEffectMode.SetActive(true);
                panelControlSCBlue.SetActive(false);
            }

            public void PCSCB_ButtonReset()
            {
                primaryColor.text = colorStr[0];
                effectMode.text = effectStr[0];
                secondaryColor.text = "None";
                instructionText.text = instructionStr[0];
                panelControlMain.SetActive(true);
                panelControlSCBlue.SetActive(false);
                WeaponRoomDisplayColor(255,0,0,255,255);
            }
        #endregion
        //End Secondary Color Blue Control Panel 

        //Secondary Color Cyan Control Panel
        #region Secondary Color Cyan Control Panel
            public void PCSCC_ButtonBlue()
            {
                secondaryColor.text = colorStr[1];
            }

            public void PCSCC_ButtonLime()
            {
                secondaryColor.text = colorStr[3];
            }

            public void PCSCC_ButtonPurple()
            {
                secondaryColor.text = colorStr[4];
            }

            public void PCSCC_ButtonBack()
            {
                instructionText.text = instructionStr[1];
                panelControlEffectMode.SetActive(true);
                panelControlSCCyan.SetActive(false);
            }

            public void PCSCC_ButtonReset()
            {
                primaryColor.text = colorStr[0];
                effectMode.text = effectStr[0];
                secondaryColor.text = "None";
                instructionText.text = instructionStr[0];
                panelControlMain.SetActive(true);
                panelControlSCCyan.SetActive(false);
                WeaponRoomDisplayColor(255,0,0,255,255);
            }
        #endregion
        //End Secondary Color Cyan Control Panel 

        //Secondary Color Lime Control Panel
        #region Secondary Color Lime Control Panel
            public void PCSCL_ButtonBlue()
            {
                secondaryColor.text = colorStr[1];
            }

            public void PCSCL_ButtonCyan()
            {
                secondaryColor.text = colorStr[2];
            }

            public void PCSCL_ButtonPurple()
            {
                secondaryColor.text = colorStr[4];
            }

            public void PCSCL_ButtonBack()
            {
                instructionText.text = instructionStr[1];
		        panelControlEffectMode.SetActive(true);
                panelControlSCLime.SetActive(false);
            }

            public void PCSCL_ButtonReset()
            {
                primaryColor.text = colorStr[0];
                effectMode.text = effectStr[0];
                secondaryColor.text = "None";
                instructionText.text = instructionStr[0];
                panelControlMain.SetActive(true);
                panelControlSCLime.SetActive(false);
                WeaponRoomDisplayColor(255,0,0,255,255);
            }
        #endregion
        //End Secondary Color Lime Control Panel 

        //Secondary Color Purple Control Panel
        #region Secondary Color Purple Control Panel
            public void PCSCP_ButtonBlue()
            {
                secondaryColor.text = colorStr[1];
            }

            public void PCSCP_ButtonCyan()
            {
                secondaryColor.text = colorStr[2];
            }

            public void PCSCP_ButtonLime()
            {
                secondaryColor.text = colorStr[3];
            }

            public void PCSCP_ButtonBack()
            {
                instructionText.text = instructionStr[1];
                panelControlEffectMode.SetActive(true);
                panelControlSCPurple.SetActive(false);
            }

            public void PCSCP_ButtonReset()
            {
                primaryColor.text = colorStr[0];
                effectMode.text = effectStr[0];
                secondaryColor.text = "None";
                instructionText.text = instructionStr[0];
                panelControlMain.SetActive(true);
                panelControlSCPurple.SetActive(false);
                WeaponRoomDisplayColor(255,0,0,255,255);
            }
        #endregion
        //End Secondary Color Purple Control Panel
    #endregion
    //End of Secondary Color Control Panel

    //Weapon Room Display Color Manager
    private void WeaponRoomDisplayColor(byte r, byte g, byte b, byte ar, byte ad)
    {
        colorRoom = new Color32(r,g,b,ar);
        colorDisplay = new Color32(r,g,b,ad);
        weaponsRoom.GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", colorRoom);
        weaponsRoom.GetComponent<MeshRenderer>().materials[1].SetColor("_EmissionColor", colorRoom);
        for(int i = 0; i < displays.Length; i++)
        {
            displays[i].GetComponent<MeshRenderer>().materials[0].SetColor("_Color", colorDisplay);
        }
    }
    //End of Weapon Room Display Color Manager

    //UI Color Manager
    private void UIColor(byte rp, byte gp, byte bp, byte ap, byte rb, byte gb, byte bb, byte ab)
    {
        uiPanelColor = new Color32(rp,gp,bp,ap);
        uiButtonColor = new Color32(rb,gb,bb,ab);

        panelTop.GetComponent<Image>().color = uiPanelColor;
        panelMiddleTop.GetComponent<Image>().color = uiPanelColor;
        panelBottom.GetComponent<Image>().color = uiPanelColor;

        for(int i = 0; i< buttonColor.Length; i++)
        {
            buttonColor[i].GetComponent<Image>().color = uiButtonColor;
        }
    }
    //End UI Color Manager
    
    //From Primary Color To Secondary Color
    private void PrimColorToSecColor()
    {
        if(primaryColor.text == colorStr[1])
        {
            panelControlSCBlue.SetActive(true);
        }
        else if(primaryColor.text == colorStr[2])
        {
            panelControlSCCyan.SetActive(true);
        }
        else if(primaryColor.text == colorStr[3])
        {
            panelControlSCLime.SetActive(true);
        }
        else if(primaryColor.text == colorStr[4])
        {
            panelControlSCPurple.SetActive(true);
        }
    }
    //End of From Primary Color To Secondary Color
}
