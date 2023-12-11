using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;

namespace TutorialMod
{
    public class VehicleTest : Script
    {
        public VehicleTest()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void OnTick(object sender, EventArgs e)
        {

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Delete current vehicle when R and control is pressed
            // This works.
            Ped character = Game.Player.Character;
            if (e.KeyCode == Keys.R && e.Control)
            {
                if (character.IsInVehicle()) 
                {
                    Vehicle currentVehicle = character.CurrentVehicle;
                    var currentAccel = currentVehicle.Acceleration;

                } else
                {
                    Notification.Show("You are not in a vehicle!");
                }
            }
        }

    }
}
