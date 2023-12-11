using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;

namespace NucleiLite
{
    public class KillAll : Script
    {
        // Set this to kill all peds but the current player.
        public KillAll() 
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void KillPeds()
        {

        }

        private void OnTick(object sender, EventArgs e)
        {

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // R and control
            /* Disabled, using this keybind in Vehicles class
            if(e.KeyCode == Keys.R && e.Control)
            {
                World.GetAllPeds();
            }
            */
            
        }

    }
}
