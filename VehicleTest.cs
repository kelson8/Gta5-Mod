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
            // Will this work?
            /*
            Ped character = Game.Player.Character;
            if(character.IsInVehicle())
            {
                Vehicle currentVehicle = character.CurrentVehicle;
                //var currentAccel = currentVehicle.Acceleration;
                //https://docs.fivem.net/natives/?_0xD5037BA82E12416F
                var currentAccelKph = currentVehicle.Acceleration * 3.6;
                var currentAccelMph = currentVehicle.Acceleration * 2.236936;
                GTA.UI.Screen.ShowHelpText($"Current speed: {currentAccelMph}mph");
            }*/
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // TODO Setup this to run and show up a ui with the current speed in the center of the screen.
            // Test this tomorrow.
            // This doesn't seem to work like this, I didn't figure it would.
            /*
            Ped character = Game.Player.Character;
            if (e.KeyCode == Keys.R && e.Control)
            {
                if (character.IsInVehicle()) 
                {
                    Vehicle currentVehicle = character.CurrentVehicle;
                    //var currentAccel = currentVehicle.Acceleration;
                    //https://docs.fivem.net/natives/?_0xD5037BA82E12416F
                    var currentAccelKph = currentVehicle.Acceleration * 3.6;
                    var currentAccelMph = currentVehicle.Acceleration * 2.236936;
                    Notification.Show("Your current speed: " + currentAccelMph + "mph");

                } else
                {
                    Notification.Show("You are not in a vehicle!");
                }
            }*/
        }

    }
}
