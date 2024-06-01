using GTA;
using GTA.UI;
using KCNetGTAV.misc;
using KCNetGTAV.teleport;
using KCNetGTAV.util;
using KCNetGTAV.vehicles;
using LemonUI;
using LemonUI.Menus;
using KCNetGTAV.players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCNetGTAV.vehicles
{
    public class VehicleSpawner
    {
        // I couldn't get the vehicle spawner to work in this class.
        // Incomplete!


        public NativeMenu vehicleSpawnerMenu = new NativeMenu("KCNet-GTA5", "Vehicle Spawner Menu");

        private readonly PlayerMenu playerMenu = new PlayerMenu();

        public void CreateVehicleSpawnerMenu()
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
    }
}
