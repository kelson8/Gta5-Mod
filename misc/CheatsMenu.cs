using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;

namespace KCNetGTAV.misc
{
    public class CheatsMenu
    {
        public NativeMenu cheatsMenu = new NativeMenu("KCNet-GTA5", "Cheat menu");
        public void CreateCheatMenu()
        {
            // They are using floats in Menyoo and defining values with strings too
            // https://github.com/MAFINS/MenyooSP/blob/master/Solution/source/Submenus/WeatherOptions.cpp#L37

            // Now that I possibly got this sorted, how do I add it to the listItemGravityLevel.
            // Not sure if this is what I would need to use.
            SortedDictionary<string, int> gravityList = new SortedDictionary<string, int>
            {
                { "Normal Gravity", 0 },
                {"Grav1", 1 },
                {"Grav2", 2 },
                {"Grav3", 3 }
            };

            // I got this part of it working
            // First time using functions with a hash value like this.
            // TODO Add what each gravity level is in the list (0, default) (1, ...) (2 ...) (3 ...)
            NativeListItem<int> listItemGravityLevel = new NativeListItem<int>("Gravity:", "Sets your gravity level", 0, 1, 2, 3);
            // I wonder if this function works with floats.
            //NativeListItem<float> listItemGravityLevel = new NativeListItem<float>("Gravity:", "Sets your gravity level", 0.008f, 0.015f, 0.1f, 0.2f);
            listItemGravityLevel.ItemChanged += (sender, args) =>
            {

                Function.Call(Hash.SET_GRAVITY_LEVEL, args.Object);
                Notification.Show($"Your gravity has been set to {args.Object}");
            };
            cheatsMenu.Add(listItemGravityLevel);
        }
    }
}
