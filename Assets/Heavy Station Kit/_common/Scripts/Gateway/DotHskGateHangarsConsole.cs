// ---------------------------------------------
// Sci-Fi Heavy Station Kit 
// Copyright (c) DotTeam. All Rights Reserved.
// https://dotteam.xyz, info@dotteam.xyz
// ---------------------------------------------

using UnityEngine;

namespace DotTeam.HSK
{

    [ExecuteInEditMode]
    public class DotHskGateHangarsConsole : MonoBehaviour
    {

        public DotHskGate gate;
        public Texture openTip;
        public Texture closeTip;
        public DotAnimatedTexture AnimatedTextureScript;

        private bool _operate = false;
        private dotHskGateMode _prevMode;

        private DotControlCenter ccInstance = null;
        private KeyCode interactShortcut = KeyCode.E;

        void Start()
        {
            if ((gate == null) || (AnimatedTextureScript == null))
            {
                //Debug.LogWarning("Controlled Gate property not assigned in Inspector or DotAnimatedTexture not attached");
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
            if (gate != null)
            {
                if (ccInstance != null) { UpdateConfig(ccInstance); }
                if (_prevMode != gate.mode)
                {
                    UpdateAnimationScreenMode();
                    _prevMode = gate.mode;
                }
                if (_operate && Input.GetKey(interactShortcut) && gate.isStopped && ((gate.mode == dotHskGateMode.active) || (gate.mode == dotHskGateMode.activeOpen)))
                {
                    gate.setState(gate.isFullyOpen, false, false);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (Common.CollideWithPlayer(other)) { _operate = (gate != null); }
        }

        void OnTriggerExit(Collider other)
        {
            if (Common.CollideWithPlayer(other)) { _operate = false; }
        }

        void OnGUI()
        {
            if (_operate && gate.isStopped && ((gate.mode == dotHskGateMode.active) || (gate.mode == dotHskGateMode.activeOpen)))
            {
                float _tw = openTip.width;
                float _th = openTip.height;
                GUI.DrawTexture(new Rect((Screen.width - _tw) / 2, Screen.height - 36 - _th, _tw, _th), gate.isFullyOpen ? closeTip : openTip, ScaleMode.ScaleToFit, true);
            }
        }

        private void UpdateAnimationScreenMode()
        {
            if (gate != null)
            {
                int seq = 0;
                switch (gate.mode)
                {
                    case dotHskGateMode.active:
                    case dotHskGateMode.activeOpen: seq = 0; break;
                    case dotHskGateMode.blocked: seq = 2; break;
                    default: seq = (((int)gate.mode & (int)dotHskGateStats.broken) != 0) ? 3 : 1; break;
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