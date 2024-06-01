using GTA;
using GTA.UI;
using KCNetGTAV.misc;
using KCNetGTAV.players;
using KCNetGTAV.teleport;
using KCNetGTAV.util;
using LemonUI;
using LemonUI.Menus;

namespace KCNetGTAV.players
{
    public class PlayerMenu
    {
        // Incomplete
        public NativeMenu playerMenu = new NativeMenu("KCNet-GTA5", "Player Menu");
        private bool isNeverWanted = false;
        private bool spawnIntoVehicle = false;
        private bool canSuperJump = false;
        private bool isExplosiveMeleeEnabled = false;

        // Getters
        // https://stackoverflow.com/questions/11159438/looking-for-a-short-simple-example-of-getters-setters-in-c-sharp
        public bool GetNeverWanted()
        {
            return isNeverWanted;
        }

        public bool GetSpawnIntoVehicle()
        {
            return spawnIntoVehicle;
        }

        public bool GetCanSuperJump()
        {
            return canSuperJump;
        }
        public bool GetExplosiveMeleeEnabled() 
        {
            return isExplosiveMeleeEnabled;
        }

        // Setters
        public void SetNeverWanted(bool value)
        {
            isNeverWanted = value;
        }

        public void SetCanSuperJump(bool value)
        {
            canSuperJump = value;
        }

        public void SetSpawnIntoVehicle(bool value)
        {
            spawnIntoVehicle = value;
        }

        public void SetExplosiveMeleeEnabled(bool value)
        {
            isExplosiveMeleeEnabled = value;
        }


        public void CreatePlayerMenu()
        {
            // TODO Add force field option (Blows up cars around player and push them away.)

            // Fix player
            NativeItem itemHealPlayer = new NativeItem("Heal player", "Restores player's health and armor");
            // This is the equivalent to:
            // private void FixPlayer(object sender, EventArgs e)
            itemHealPlayer.Activated += (sender, args) =>
            {
                Game.Player.Character.Health = Game.Player.Character.MaxHealth;
                Game.Player.Character.Armor = Game.Player.MaxArmor;
                Notification.Show("Health and armor restored!");
            };
            playerMenu.Add(itemHealPlayer);

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
                spawnIntoVehicle = checkBoxSpawnIntoVehicle.Checked;
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
    }
}
