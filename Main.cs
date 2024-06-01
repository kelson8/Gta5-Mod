// This could be useful for enabling test code, I always forget C# has preprocessors.
//#define _TEST
// I haven't figured out how to get the vehicle spawner working in another class so I disabeld it using #if !_TEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using KCNetGTAV.misc;
using KCNetGTAV.players;
using KCNetGTAV.teleport;
using KCNetGTAV.util;
using KCNetGTAV.vehicles;
using LemonUI;
using LemonUI.Menus;


namespace KCNetGTAV
{

    // Most of this code came from this guide: https://github.com/KimonoBoy/SHVDNTutorial-KCNet-GTA5/wiki/Developing-our-Mod-Menu#creating-our-menu-using-lemonui

    // TODO store set values into a config file, so invincibility, never wanted and stuff like that stays on.
    // Currently it resets when the menu reloads.
    // TODO Figure out how to create submenus nested within submenus, like a teleport submenu
    // Use xml for storing stuff if needed.

    // TODO Look into this sometime (Draw text on screen): https://gtaforums.com/topic/945754-solved-drawing-text-on-screen-c-scripthookvdotnet/

    /* Place holder for empty pages:
    NativeItem blankItem = new NativeItem("Not setup!", "This page hasn't been created yet!");
    cheatsMenu.Add(blankItem);
    */

    // Dubai Highway start coords:
    // X: 4965.44, Y: -2935.58, Z: 21.00, Heading 4.66

    public class Main : Script
    {
        // Other classes
        PedChanger pedChanger = new PedChanger();
        CheatsMenu cheatsMenu = new CheatsMenu();
        TeleportMenu teleportMenu = new TeleportMenu();
        TestMenu testMenu = new TestMenu();
        PlayerMenu playerMenu = new PlayerMenu();


#if _TEST
        VehicleSpawner vehicleSpawner = new VehicleSpawner();
#endif

#if !_TEST
        NativeMenu vehicleSpawnerMenu = new NativeMenu("KCNet-GTA5", "Vehicle Spawner Menu");
#endif

        VehicleOptions vehicleOptions = new VehicleOptions();

        ObjectPool menuPool = new ObjectPool();
        // Main menu
        NativeMenu mainMenu = new NativeMenu("KCNet-GTA5", "Main Menu");
        // Sub menus
        //NativeMenu playerMenu = new NativeMenu("KCNet-GTA5", "Player Menu");
        //NativeMenu vehicleSpawnerMenu = new NativeMenu("KCNet-GTA5", "Vehicle Spawner Menu");
        NativeMenu vehicleOptionsMenu = new NativeMenu("KCNet-GTA5", "Vehicle Options Menu");
        NativeMenu weaponsMenu = new NativeMenu("KCNet-GTA5", "Weapons Menu");
        //NativeMenu cheatsMenu = new NativeMenu("KCNet-GTA5", "Cheat menu");
        //NativeMenu teleportMenu = new NativeMenu("KCNet-GTA5", "Teleport menu");

        //NativeMenu pedChangerMenu = new NativeMenu("KCNet-GTA5", "Change model");
        //NativeMenu testMenu = new NativeMenu("KCNet-GTA5", "Test Menu");

        //bool canSuperJump = false;
        //bool isNeverWanted = false;
        //bool SpawnIntoVehicle = false;
        bool isFlamingBulletsEnabled = false;
        bool isExplosiveBulletsEnabled = false;
        //bool isExplosiveMeleeEnabled = false;

        //bool isMobileRadioActive = false;

        public Main()
        {            
            CreateMainMenu();
            //CreatePlayerMenu();
            playerMenu.CreatePlayerMenu();
#if _TEST
            vehicleSpawner.CreateVehicleSpawnerMenu();
#endif //_TEST
#if !_TEST
            CreateVehicleSpawnerMenu();
#endif //!_TEST


            CreateVehicleOptionsMenu();
            CreateWeaponsMenu();
            //CreateTestMenu();

            // New classes
            cheatsMenu.CreateCheatMenu();
            // Implement this
            //PedMenu.CreatePedChangerMenu();
            pedChanger.CreatePedChangerMenu();
            teleportMenu.CreateTeleportMenu();
            testMenu.CreateTestMenu();
            //playerMenu.CreatePlayerMenu();

            AddMenusToPool();

            KeyDown += OnKeyDown;
            Tick += OnTick;
        }

        private void CreateMainMenu()
        {
            // Define sub menus under mainMenu
            //mainMenu.AddSubMenu(playerMenu);
            mainMenu.AddSubMenu(playerMenu.playerMenu);
#if _TEST
            mainMenu.AddSubMenu(vehicleSpawner.vehicleSpawnerMenu);
#endif //_TEST

#if !_TEST
            mainMenu.AddSubMenu(vehicleSpawnerMenu);
#endif

            //mainMenu.AddSubMenu(vehicleSpawner);

            mainMenu.AddSubMenu(vehicleOptionsMenu);
            mainMenu.AddSubMenu(weaponsMenu);
            
            //mainMenu.AddSubMenu(testMenu);
            // New classes
            mainMenu.AddSubMenu(cheatsMenu.cheatsMenu);
            mainMenu.AddSubMenu(pedChanger.pedChangerMenu);
            mainMenu.AddSubMenu(teleportMenu.teleportMenu);
            mainMenu.AddSubMenu(testMenu.testMenu);
            //mainMenu.AddSubMenu(playerMenu.playerMenu);
        }

        private void CreateWeaponsMenu()
        {
            // TODO Add menus for weapons and their classes.
            NativeItem itemGiveAllWeapons = new NativeItem("Give All Weapons", "Gives the Player all Weapons.");
            itemGiveAllWeapons.Activated += (sender, args) =>
            {
                Ped character = Game.Player.Character;
                foreach (WeaponHash weaponHash in Enum.GetValues(typeof(WeaponHash)))
                {
                    character.Weapons.Give(weaponHash, 100, false, true);
                    character.Weapons[weaponHash].Ammo = character.Weapons[weaponHash].MaxAmmo;
                }
                Notification.Show("Player gained all weapons with max ammunition.");
            };
            weaponsMenu.Add(itemGiveAllWeapons);

            NativeCheckboxItem checkBoxBulletsFire = new NativeCheckboxItem("Flaming bullets");
            checkBoxBulletsFire.CheckboxChanged += (sender, args) =>
            {
                isFlamingBulletsEnabled = checkBoxBulletsFire.Checked;
            };
            weaponsMenu.Add(checkBoxBulletsFire);

            NativeCheckboxItem checkBoxBulletsExplosive = new NativeCheckboxItem("Explosive Bullets");
            checkBoxBulletsExplosive.CheckboxChanged += (sender, args) =>
            {
                isExplosiveBulletsEnabled = checkBoxBulletsExplosive.Checked;
            };
            weaponsMenu.Add(checkBoxBulletsExplosive);
        }

#if !_TEST

        private void CreateVehicleSpawnerMenu()
        {
            foreach (VehicleHash vehicleHash in Enum.GetValues(typeof(VehicleHash)))
            {
                NativeItem itemSpawnVehicle = new NativeItem(vehicleHash.ToString(), $"Spawns a {vehicleHash} right in front of you!");

                itemSpawnVehicle.Activated += (sender, args) =>
                {
                    Ped character = Game.Player.Character;


                    Model vehicleModel = new Model(vehicleHash);
                    vehicleModel.Request();

                    //TODO Try and figure out Random vehicle spawning:
                    // World.CreateRandomVehicle(spawnPosition, vehicleModel);

                    //TODO Add submenu for vehicle classes
                    // TODO Make this code only spawn set amount of vehicles in a config.
                    // So if it's set to 2, set max amount of spawned cars at a time to 2.
                    Vehicle vehicle = World.CreateVehicle(vehicleModel, character.Position + character.ForwardVector * 3.0f, character.Heading + 90.0f);


                    //if (SpawnIntoVehicle)
                    if (playerMenu.GetSpawnIntoVehicle())
                    {
                        // If player is in vehicle, remove old one before spawning in new
                        // Get current vehicle and delete it.
                        // TODO Fix this to where it doesn't rotate the vehicle if player is in a vehicle.
                        if (character.IsInVehicle())
                        {
                            Vehicle currentVehicle = character.CurrentVehicle;
                            currentVehicle.Delete();
                            character.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                        }
                        else
                        {
                            character.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                        }

                    }

                    vehicleModel.MarkAsNoLongerNeeded();

                    Notification.Show($"Vehicle {vehicleHash} has been spawned!");
                };
                vehicleSpawnerMenu.Add(itemSpawnVehicle);
            }
        }
#endif //!_TEST

        private void CreateVehicleOptionsMenu()
        {
            var player = Game.Player;
            var playerchar = player.Character;

            NativeItem fixVehicleItem = new NativeItem("Repair vehicle", "Fix your current vehicle.");

            fixVehicleItem.Activated += (sender, args) =>
            {
                if (playerchar.IsInVehicle())
                {
                    Vehicle veh = playerchar.CurrentVehicle;
                    veh.Repair();
                    Notification.Show("Your vehicle has been ~y~repaired~w~!");
                }
                else
                {
                    Notification.Show("~r~Error~w~: This only works on vehicles!");
                }
            };
            vehicleOptionsMenu.Add(fixVehicleItem);
            
        }

        // 3-29-2024 @ 4:18AM
    //    private void CreateTestMenu()
    //    {
    //        // I tested this and it works fine. 3-29-2024 @ 4:34AM
    //        NativeCheckboxItem toggleMobileRadioItem = new NativeCheckboxItem("Toggle Mobile Radio");

    //    toggleMobileRadioItem.CheckboxChanged += (sender, args) =>
    //        {
    //            // https://github.com/scripthookvdotnet/scripthookvdotnet/issues/179
    //            isMobileRadioActive = toggleMobileRadioItem.Checked;
    //            Function.Call(Hash.SET_MOBILE_PHONE_RADIO_STATE, isMobileRadioActive);
    //            Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, isMobileRadioActive);
    //        };
    ////testMenu.Add(toggleMobileRadioItem);
    //    }

private void AddMenusToPool()
        {
            // Main menu and submenus go under menuPool
            menuPool.Add(mainMenu);
            menuPool.Add(playerMenu.playerMenu);
            //menuPool.Add(playerMenu);
#if _TEST
            mainMenu.AddSubMenu(vehicleSpawner.vehicleSpawnerMenu);
#endif
#if !_TEST
            menuPool.Add(vehicleSpawnerMenu);
#endif

            menuPool.Add(vehicleOptionsMenu);
            //menuPool.Add(vehicleOptions.vehicleOptionsMenu);

            menuPool.Add(weaponsMenu);
            //menuPool.Add(cheatsMenu);
            //menuPool.Add(teleportMenu);
            //menuPool.Add(teleportMenu);
            // Will this work?
            //menuPool.Add(pedChangerMenu);
            //menuPool.Add(testMenu);

            // New classes
            menuPool.Add(cheatsMenu.cheatsMenu);
            menuPool.Add(pedChanger.pedChangerMenu);
            menuPool.Add(teleportMenu.teleportMenu);
            menuPool.Add(testMenu.testMenu);
        }


        private void OnTick(object sender, EventArgs e)
        {
            // Needed for the Menu to function
            menuPool.Process();

            if(playerMenu.GetNeverWanted() && Game.Player.WantedLevel > 0)
            {
                Game.Player.WantedLevel = 0;
            }

            if (playerMenu.GetCanSuperJump())
            {
                Game.Player.SetSuperJumpThisFrame();
            }

            if (playerMenu.GetExplosiveMeleeEnabled())
            {
                Game.Player.SetExplosiveMeleeThisFrame();
            }

            if (isFlamingBulletsEnabled)
            {
                Game.Player.SetFireAmmoThisFrame();
            }

            if(isExplosiveBulletsEnabled)
            {
                Game.Player.SetExplosiveAmmoThisFrame();
            }

            /*
             * Try to add a couple of things like this to the menu.
            if (isFastRunEnabled)
            {
                Game.Player.SetRunSpeedMultThisFrame(amount);
            }*/
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }
    }
}