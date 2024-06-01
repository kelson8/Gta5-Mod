using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using KCNetGTAV.misc;
using KCNetGTAV.players;
using LemonUI;
using LemonUI.Menus;

namespace KCNetGTAV.teleport
{
    public class TeleportMenu
    {
        public NativeMenu teleportMenu = new NativeMenu("KCNet-GTA5", "Teleport menu");

        public void CreateTeleportMenu()
        {
            NativeItem itemTeleportWaypoint = new NativeItem("Waypoint", "Teleports you to the waypoint on the map");

            //TODO Fix this to where it doesn't put the player/vehicle under the world.
            //TODO Fix this to where it isn't a bunch of duplicated code for every teleport option.
            itemTeleportWaypoint.Activated += (sender, args) =>
            {
                if (Game.IsWaypointActive)
                {
                    var player = Game.Player;
                    var playerchar = player.Character;
                    Vector3 pos = World.WaypointPosition;

                    if (playerchar.IsInVehicle())
                    {

                        // Changing this to player.getHashCode() Makes it to where I can teleport the car.
                        Function.Call(Hash.START_PLAYER_TELEPORT, player.GetHashCode(), pos.X, pos.Y + 10, pos.Z, 0.0, true, false, true);

                        // This might work for vehicles, I couldn't get it working like this though.
                        //Function.Call(Hash.START_PLAYER_TELEPORT, player.GetHashCode(), pos.X, pos.Y + 10, pos.Z, 0.0, false, false, true);

                        Notification.Show("You teleported your car to the ~p~marker~w~!");
                        // Get last vehicle player was in.
                        //Vehicle vehicle = playerchar.CurrentVehicle;
                        //Vehicle lastVeh = playerchar.LastVehicle;
                        //lastVeh.Delete();

                    }
                    else
                    {

                        // https://github.com/scripthookvdotnet/scripthookvdotnet/wiki/How-Tos#calling-native-functions
                        // Testing set entity coords
                        // Functions might be useful if I can figure out how to use them, this wasn't working but I fixed it.
                        //Function.Call(Hash.START_PLAYER_TELEPORT, player.GetHashCode(), pos.X, pos.Y + 10, pos.Z, 0.0, false, false, true);

                        // Use this anytime you want to teleport the player.
                        // TODO figure out how to set the coordinates invidually so i can do "pos.Y + 5"
                        // Something like this below might work, but it complains about not being a Vector3 if used.
                        // float targetPosX = pos.X;
                        var zCoords = World.GetGroundHeight(pos);
                        Function.Call(Hash.START_PLAYER_TELEPORT, player.GetHashCode(), pos.X, pos.Y, zCoords, 0.0, false, false, true);

                        //Game.Player.Character.Position = pos;

                        // Player still falls through ground in certain areas with this below.
                        //Function.Call(Hash.START_PLAYER_TELEPORT, player.GetHashCode(), pos.X, pos.Y + 10, pos.Z, 0.0, false, false, true);

                        Notification.Show("You have been teleported to the ~p~marker~w~!");
                    }
                }
                else
                {
                    Notification.Show("~r~Error~w~: no waypoint is set!");
                }
            };
            teleportMenu.Add(itemTeleportWaypoint);
        }

    }
}
