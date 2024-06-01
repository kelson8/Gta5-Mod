using System;

using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;

namespace KCNetGTAV.players
{
    public class PedChanger : Script
    {
        // I got this working in a different class 6-1-2024 @ 3:07AM

        public NativeMenu pedChangerMenu = new NativeMenu("KCNet-GTA5", "Change model");
        public void CreatePedChangerMenu()
        {
            // TODO Add different categories
            foreach (PedHash pedHash in Enum.GetValues(typeof(PedHash)))
            {
                NativeItem itemChangePed = new NativeItem(pedHash.ToString(), "Changes your skin");

                itemChangePed.Activated += (sender, args) =>
                {
                    var characterModel = new Model(pedHash);
                    characterModel.Request(500);

                    // If the model isn't loaded, wait until it is.
                    while (!characterModel.IsLoaded) Script.Wait(100);

                    Game.Player.ChangeModel(characterModel);
                    Notification.Show($"You have changed your character.");

                    characterModel.MarkAsNoLongerNeeded();

                };
                pedChangerMenu.Add(itemChangePed);
            }
        }
    }
}
