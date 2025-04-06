using System;
using System.Windows.Forms;
using GTA;
using GTA.UI;

namespace VWeaponEditor.SHVDN3 {
    public class VWeaponEditorScript : Script {
        private ulong tick;
        
        public VWeaponEditorScript() {
            this.KeyDown += this.OnKeyDown;
            this.KeyUp += this.OnKeyUp;
            this.Tick += this.OnTick;
        }

        private void OnTick(object sender, EventArgs e) {
            if (++this.tick == 1) {
                Notification.Show("~g~VWeaponEditor~w~ loaded!");
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e) {
            
        }
        
        private void OnKeyUp(object sender, KeyEventArgs e) {
            
        }
    }
}