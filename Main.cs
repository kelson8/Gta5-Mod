using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;

namespace TutorialMod
{

    // Most of this code came from this guide: https://github.com/KimonoBoy/SHVDNTutorial-NucleiLite/wiki/Developing-our-Mod-Menu#creating-our-menu-using-lemonui

    // TODO store set values into a config file, so invincibility, never wanted and stuff like that stays on.
    // Currently it resets when the menu reloads.
    // TODO Figure out how to create submenus nested within submenus, like a teleport submenu
    // Use xml for storing stuff if needed.

    /* Place holder for empty pages:
    NativeItem blankItem = new NativeItem("Not setup!", "This page hasn't been created yet!");
    cheatsMenu.Add(blankItem);
    */

    public class Main : Script
    {
        ObjectPool menuPool = new ObjectPool();
        // Main menu
        NativeMenu mainMenu = new NativeMenu("NucleiLite", "Main Menu");
        // Sub menus
        NativeMenu playerMenu = new NativeMenu("NucleiLite", "Player Menu");
        NativeMenu vehicleSpawnerMenu = new NativeMenu("NucleiLite", "Vehicle Spawner Menu");
        NativeMenu weaponsMenu = new NativeMenu("NucleiLite", "Weapons Menu");
        NativeMenu cheatsMenu = new NativeMenu("NucleiLite", "Cheat menu");
        NativeMenu teleportMenu = new NativeMenu("NucleiLite", "Teleport menu");
        NativeMenu pedChangerMenu = new NativeMenu("NucleiLite", "Change model");

        bool canSuperJump = false;
        bool isNeverWanted = false;
        bool SpawnIntoVehicle = false;
        bool isFlamingBulletsEnabled = false;
        bool isExplosiveBulletsEnabled = false;
        bool isExplosiveMeleeEnabled = false;

        public Main()
        {
            CreateMainMenu();
            CreatePlayerMenu();
            CreateVehicleSpawnerMenu();
            CreateWeaponsMenu();
            CreateCheatMenu();
            CreateTeleportMenu();
            CreatePedChangerMenu();

            AddMenusToPool();

            KeyDown += OnKeyDown;
            Tick += OnTick;
        }

        private void CreateMainMenu()
        {
            // Define sub menus under mainMenu
            mainMenu.AddSubMenu(playerMenu);
            mainMenu.AddSubMenu(vehicleSpawnerMenu);
            mainMenu.AddSubMenu(weaponsMenu);
            mainMenu.AddSubMenu(cheatsMenu);
            mainMenu.AddSubMenu(teleportMenu);
            mainMenu.AddSubMenu(pedChangerMenu);
        }

        private void CreatePlayerMenu()
        {
            // TODO Add force field option (Blows up cars around player and push them away.)

            // Fix player
            NativeItem itemFixPlayer = new NativeItem("Fix player", "Restores player's health and armor");
            // This is the equivalent to:
            // private void FixPlayer(object sender, EventArgs e)
            itemFixPlayer.Activated += (sender, args) =>
            {
                Game.Player.Character.Health = Game.Player.Character.MaxHealth;
                Game.Player.Character.Armor = Game.Player.MaxArmor;
                Notification.Show("Health and armor restored!");
            };
            playerMenu.Add(itemFixPlayer);

            // Invincible
            NativeCheckboxItem checkBoxInvincible = new NativeCheckboxItem("Invincible", "Gives you god mode");
            checkBoxInvincible.CheckboxChanged += (sender, args) =>
            {
                Game.Player.Character.IsInvincible = checkBoxInvincible.Checked;
                // Equivalent of doing this: ("IsInvincible: " + Game.Player.Character.IsInvincible.ToString())
                Notification.Show($"Invincible: {Game.Player.Character.IsInvincible}");
            };
            playerMenu.Add(checkBoxInvincible);

            // Wanted level
            NativeListItem<int> listItemWantedLevel = new NativeListItem<int>("Wanted Level", "Adjust player's wanted level", 0, 1, 2, 3, 4, 5);
            listItemWantedLevel.ItemChanged += (sender, args) =>
            {
                Game.Player.WantedLevel = args.Object;
            };
            playerMenu.Add(listItemWantedLevel);

            // Never wanted, Well it did work, I just copied what I was doing for the super jump code.
            NativeCheckboxItem checkboxNeverWanted = new NativeCheckboxItem("Never Wanted");
            checkboxNeverWanted.CheckboxChanged += (sender, args) =>
            {
                isNeverWanted = checkboxNeverWanted.Checked;
            };
            playerMenu.Add(checkboxNeverWanted);

            // Spawn player in car
            NativeCheckboxItem checkBoxSpawnIntoVehicle = new NativeCheckboxItem("Spawn into car", "This option sets you to spawn in the car instead of out of it.");
            checkBoxSpawnIntoVehicle.CheckboxChanged += (sender, args) =>
            {
                SpawnIntoVehicle = checkBoxSpawnIntoVehicle.Checked;
            };
            playerMenu.Add(checkBoxSpawnIntoVehicle);

            /* I can also use NativeListItem with string like so
             NativeListItem<string> listItemHairColor = new NativeListItem<string>("Hair Color", "Change Hair Color", "Black", "White", "Blue", "Green");
            listItemHairColor.ItemChanged += (sender, args) =>
            {
                Notification.Show($"You've selected the Hair Color: {args.Object}");
            };
            playerMenu.Add(listItemHairColor);
            */

            // Super jump
            NativeCheckboxItem checkBoxSuperJump = new NativeCheckboxItem("Super jump");
            checkBoxSuperJump.CheckboxChanged += (sender, args) =>
            {
                canSuperJump = checkBoxSuperJump.Checked;
            };
            playerMenu.Add(checkBoxSuperJump);

            // Explosive melee
            NativeCheckboxItem checkBoxExplosiveMelee = new NativeCheckboxItem("Explosive melee");
            checkBoxExplosiveMelee.CheckboxChanged += (sender, args) =>
            {
                isExplosiveMeleeEnabled = checkBoxExplosiveMelee.Checked;
            };
            playerMenu.Add(checkBoxExplosiveMelee);
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

        private void CreateVehicleSpawnerMenu()
        {
            foreach(VehicleHash vehicleHash in Enum.GetValues(typeof(VehicleHash)))
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

                    if (SpawnIntoVehicle)
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

        private void CreateCheatMenu()
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
            listItemGravityLevel.ItemChanged += (sender, args) => {

                Function.Call(Hash.SET_GRAVITY_LEVEL, args.Object);
                Notification.Show($"Your gravity has been set to {args.Object}");
            };
            cheatsMenu.Add(listItemGravityLevel);
        }

        private void CreateTeleportMenu()
        {
            NativeItem itemTeleportWaypoint = new NativeItem("Waypoint", "Teleports you to the waypoint on the map");
            //World.WaypointPosition;

            itemTeleportWaypoint.Activated += (sender, args) =>
            {
                if (Game.IsWaypointActive)
                {
                    // Disable teleporting in vehicle, it's teleports the player without the vehicle.
                    // TODO Fix this to where it works on a vehicle too.
                    if (Game.Player.Character.IsInVehicle())
                    {
                        Notification.Show("Not implemented for vehicles yet!");
                    }
                    else
                    {
                        Vector3 pos = World.WaypointPosition;

                        // https://github.com/scripthookvdotnet/scripthookvdotnet/wiki/How-Tos#calling-native-functions
                        // Testing set entity coords
                        // Functions might be useful if I can figure out how to use them, this wasn't working but I fixed it.
                        //Function.Call(Hash.SET_ENTITY_COORDS, Game.Player.GetHashCode(), pos.X, pos.Y +5, pos.Z);

                        // Use this anytime you want to teleport the player.
                        // TODO figure out how to set the coordinates invidually so i can do "pos.Y + 5"
                        // Something like this below might work, but it complains about not being a Vector3 if used.
                        // float targetPosX = pos.X;
                        Game.Player.Character.Position = pos;

                        Notification.Show("You have been teleported to the marker");
                    }
                }
                else
                {
                    Notification.Show("Error, no waypoint is set!");
                }
            };
            teleportMenu.Add(itemTeleportWaypoint);

            /* 
            // https://github.com/scripthookvdotnet/scripthookvdotnet/wiki/How-Tos#calling-native-functions
            // Testing set entity coords
            //
            Function.Call(Hash.SET_ENTITY_COORDS, pos.X, pos.Y, pos.Z, false, false, false, false);
            */

        }

        private void CreatePedChangerMenu()
        {
            // TODO Add different categories
            foreach(PedHash pedHash in Enum.GetValues(typeof(PedHash)))
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

        private void AddMenusToPool()
        {
            // Main menu and submenus go under menuPool
            menuPool.Add(mainMenu);
            menuPool.Add(playerMenu);
            menuPool.Add(vehicleSpawnerMenu);
            menuPool.Add(weaponsMenu);
            menuPool.Add(cheatsMenu);
            menuPool.Add(teleportMenu);
            menuPool.Add(pedChangerMenu);
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Needed for the Menu to function
            menuPool.Process();

            if(isNeverWanted && Game.Player.WantedLevel > 0)
            {
                Game.Player.WantedLevel = 0;
            }

            if(canSuperJump)
            {
                Game.Player.SetSuperJumpThisFrame();
            }

            if(isExplosiveMeleeEnabled)
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