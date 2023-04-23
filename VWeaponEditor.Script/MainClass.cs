using System;
using System.Diagnostics;
using System.Windows.Forms;
using GTA;

namespace VWeaponEditor.Script {
    // was playing around with scripts but it barely works
    // managed to debug it though... can't access Game.Player off of main thread which
    // is fantastic considering there's apparently no way to "invoke" a method on the main thread
    // Debug -> Attach to process -> GTA5.exe, then you can set breakpoints as long as the DLL is up to date
    public class MainClass : GTA.Script {
        public MainClass() {
            this.KeyUp += this.MainClass_KeyUp;
        }

        private void MainClass_KeyUp(object sender, KeyEventArgs e) {
            try {
                if (e.KeyCode == Keys.NumPad7) {
                    Player player = Game.Player;
                    Vehicle veh = player.LastVehicle;
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }
    }
}