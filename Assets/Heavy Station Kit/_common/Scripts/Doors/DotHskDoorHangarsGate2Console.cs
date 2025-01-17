// ---------------------------------------------
// Sci-Fi Heavy Station Kit 
// Copyright (c) DotTeam. All Rights Reserved.
// https://dotteam.xyz, info@dotteam.xyz
// ---------------------------------------------

using UnityEngine;

namespace DotTeam.HSK
{

    [ExecuteInEditMode]
    public class DotHskDoorHangarsGate2Console : MonoBehaviour
    {

        public DotHskDoor gate2;
        public Texture openTip;
        public Texture closeTip;
        public DotAnimatedTexture AnimatedTextureScript;

        private bool _operate = false;
        private dotHskDoorMode _prevMode;

        private DotControlCenter ccInstance = null;
        private KeyCode interactShortcut = KeyCode.E;

        void Start()
        {
            if ((gate2 == null) || (AnimatedTextureScript == null))
            {
                //Debug.LogWarning("Controlled Gate2 property not assigned in Inspector or DotAnimatedTexture not attached");
            }
            UpdateAnimationScreenMode();
            // Update cofiguration 
            if (DotControlCenter.instance != null)
            {
                if (DotControlCenter.instance.trackChangesSettings) { ccInstance = DotControlCenter.instance; };
                UpdateConfig(DotControlCenter.instance);
            }
        }

        void Update()
        {
            if (gate2 != null)
            {
                // Update Configuration Changes
                if (ccInstance != null) { UpdateConfig(ccInstance); }
                if (_prevMode != gate2.mode)
                {
                    UpdateAnimationScreenMode();
                    _prevMode = gate2.mode;
                }
                if (_operate && Input.GetKey(interactShortcut) && ((gate2.mode == dotHskDoorMode.active) || (gate2.mode == dotHskDoorMode.activeOpen)) && gate2.doorScript.getIsStopped())
                {
                    gate2.doorScript.operate(gate2.doorScript.getIsFullyClosed());
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (Common.CollideWithPlayer(other)) { _operate = (gate2 != null) && (gate2.doorScript != null); }
        }

        void OnTriggerExit(Collider other)
        {
            if (Common.CollideWithPlayer(other)) { _operate = false; }
        }

        void OnGUI()
        {
            if (_operate && ((gate2.mode == dotHskDoorMode.active) || (gate2.mode == dotHskDoorMode.activeOpen)) && gate2.doorScript.getIsStopped())
            {
                float _tw = openTip.width;
                float _th = openTip.height;
                GUI.DrawTexture(new Rect((Screen.width - _tw) / 2, Screen.height - 36 - _th, _tw, _th), gate2.doorScript.getIsFullyOpen() ? closeTip : openTip, ScaleMode.ScaleToFit, true);
            }
        }

        private void UpdateAnimationScreenMode()
        {
            if (gate2 != null)
            {
                int seq = 0;
                switch (gate2.mode)
                {
                    case dotHskDoorMode.active:
                    case dotHskDoorMode.activeOpen: seq = 0; break;
                    case dotHskDoorMode.blocked: seq = 2; break;
                    default: seq = (((int)gate2.mode & (int)dotHskDoorStats.broken) != 0) ? 3 : 1; break;
                }
                AnimatedTextureScript.activeSequence = seq;
            }
        }

        void UpdateConfig(DotControlCenter c)
        {
            interactShortcut = c.interactShortcut;
        }

    }

}