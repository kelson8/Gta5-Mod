using System;
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

    // Most of this code came from this guide: https://github.com/KimonoBoy/SHVDNTutorial-NucleiLite/wiki/Developing-our-Mod-Menu#creating-our-menu-using-lemonui

    // TODO store set values into a config file, so invincibility, never wanted and stuff like that stays on.
    // Currently it resets when the menu reloads.
    // TODO Figure out how to create submenus nested within submenus, like a teleport submenu
    // Use xml for storing stuff if needed.
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
        bool NeverWanted = false;
        bool SpawnIntoVehicle = false;

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
            NativeCheckboxItem checkBoxInvincible = new NativeCheckboxItem("Invincible", "You can no longer die.");
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
            NativeCheckboxItem checkboxNeverWanted = new NativeCheckboxItem("Never Wanted", "You can no longer get cops.");
            checkboxNeverWanted.CheckboxChanged += (sender, args) =>
            {
                NeverWanted = checkboxNeverWanted.Checked;
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
            NativeCheckboxItem checkBoxSuperJump = new NativeCheckboxItem("Superjump", "Jump very high");
            checkBoxSuperJump.CheckboxChanged += (sender, args) =>
            {
                canSuperJump = checkBoxSuperJump.Checked;
            };
            playerMenu.Add(checkBoxSuperJump);
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

                    // This works, now to replace current vehicle with the new one, add check box for spawning into driver seat.
                    //character.SetIntoVehicle(vehicle, VehicleSeat.Driver);

                    // This works like this so i'll keep it here.
                    /*
                    if (SpawnIntoVehicle)
                    {
                        character.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                    }
                    */

                    if (SpawnIntoVehicle)
                    {
                        // If player is in vehicle, remove old one before spawning in new
                        // Get current vehicle and delete it.
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
            // Not implemented yet
            //NativeCheckboxItem gravityCheckBox = new NativeCheckboxItem("Gravity", "Change the gravity to set value");
            NativeItem blankItem = new NativeItem("Not setup!", "This page hasn't been created yet!");
            cheatsMenu.Add(blankItem);

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

            if (NeverWanted)
            {
                Game.Player.WantedLevel = 0;
            }

            if (canSuperJump)
            {
                Game.Player.SetSuperJumpThisFrame();
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