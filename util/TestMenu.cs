using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using KCNetGTAV.misc;
using KCNetGTAV.players;
using LemonUI;
using LemonUI.Menus;

namespace KCNetGTAV.util
{
    public class TestMenu
    {
        public NativeMenu testMenu = new NativeMenu("KCNet-GTA5", "Test Menu");
        bool isMobileRadioActive = false;
        
        //3-29-2024 @ 4:18AM
        public void CreateTestMenu()
        {
            Vector3 playerPos = Game.Player.Character.Position;

            //
            NativeCheckboxItem toggleMobileRadioItem = new NativeCheckboxItem("Toggle Mobile Radio");

            toggleMobileRadioItem.CheckboxChanged += (sender, args) =>
            {
                // https://github.com/scripthookvdotnet/scripthookvdotnet/issues/179
                isMobileRadioActive = toggleMobileRadioItem.Checked;
                Function.Call(Hash.SET_MOBILE_PHONE_RADIO_STATE, isMobileRadioActive);
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, isMobileRadioActive);
            };
            testMenu.Add(toggleMobileRadioItem);
            //

            //
            NativeItem toggleExplosion = new NativeItem("Bomb");

            toggleExplosion.Activated += (sender, args) =>
            {
                World.AddExplosion(playerPos, ExplosionType.Rocket, 10, 0.5f);
            };
            testMenu.Add(toggleExplosion);
            //

            //
            NativeSliderItem nativeSliderItem = new NativeSliderItem("Test");
            nativeSliderItem.Multiplier = 10;
            
            //Notification.Show("")

            testMenu.Add(nativeSliderItem);
            //

            
        }

    }
}
