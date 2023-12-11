using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;

namespace NucleiLite
{
    public class Main : Script
    {
        ObjectPool menuPool = new ObjectPool();
        // Main menu
        NativeMenu mainMenu = new NativeMenu("NucleiLite", "Main Menu");
        // Sub menus
        NativeMenu playerMenu = new NativeMenu("NucleiLite", "Player Menu");
        NativeMenu vehicleSpawnerMenu = new NativeMenu("NucleiLite", "Vehicle Spawner Menu");
        NativeMenu weaponsMenu = new NativeMenu("NucleiLite", "Weapons Menu");

        bool canSuperJump = false;
        bool NeverWanted = false;

        public Main()
        {
            CreateMainMenu();
            CreatePlayerMenu();
            CreateVehicleSpawnerMenu();
            CreateWeaponsMenu();

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
                    Vehicle vehicle = World.CreateVehicle(vehicleModel, character.Position + character.ForwardVector * 3.0f, character.Heading + 90.0f);

                    // This works, now to replace current vehicle with the new one, add check box for spawning into driver seat.
                    //character.SetIntoVehicle(vehicle, VehicleSeat.Driver);

                    if (character.IsInVehicle())
                    {
                        
                        vehicle.Delete();
                        // This works, now to replace current vehicle with the new one.
                        character.SetIntoVehicle(vehicle, VehicleSeat.Driver);

                        // Don't attempt to delete vehicle if player isn't currently in one.
                    } else
                    {
                        character.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                    }

                    vehicleModel.MarkAsNoLongerNeeded();

                    Notification.Show($"Vehicle {vehicleHash} has been spawned!");
                };
                vehicleSpawnerMenu.Add(itemSpawnVehicle);
            }
        }

        private void AddMenusToPool()
        {
            // Main menu and submenus go under menuPool
            menuPool.Add(mainMenu);
            menuPool.Add(playerMenu);
            menuPool.Add(vehicleSpawnerMenu);
            menuPool.Add(weaponsMenu);
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