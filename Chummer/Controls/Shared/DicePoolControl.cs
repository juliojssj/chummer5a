/*  This file is part of Chummer5a.
 *
 *  Chummer5a is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Chummer5a is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Chummer5a.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  You can obtain the full source code for Chummer5a at
 *  https://github.com/chummer5a/chummer5a
 */

using System;
using System.Windows.Forms;

namespace Chummer.UI.Shared.Components
{
    public partial class DicePoolControl : UserControl
    {
        private int _intDicePool;
        private bool _blnCanBeRolled = true;
        private bool _blnCanEverBeRolled = Utils.IsDesignerMode;

        public DicePoolControl()
        {
            InitializeComponent();
            this.UpdateLightDarkMode();
            this.TranslateWinForm();

            CanEverBeRolled = CanEverBeRolled || GlobalSettings.AllowSkillDiceRolling;
            cmdRoll.Visible = CanBeRolled && CanEverBeRolled;
        }

        private async void DicePoolControl_Load(object sender, EventArgs e)
        {
            cmdRoll.ToolTipText = await LanguageManager.GetStringAsync("Tip_DiceRoller");
        }

        private void cmdRoll_Click(object sender, EventArgs e)
        {
            if (Program.MainForm == null)
                return;
            Character objCharacter = null;
            if (ParentForm is CharacterShared frmParent)
                objCharacter = frmParent.CharacterObject;
            Program.MainForm.OpenDiceRollerWithPool(objCharacter, DicePool);
        }

        public void SetLabelToolTip(string caption)
        {
            lblDicePool.SetToolTip(caption);
        }

        public bool CanBeRolled
        {
            get => _blnCanBeRolled;
            set
            {
                if (_blnCanBeRolled == value)
                    return;
                _blnCanBeRolled = value;
                cmdRoll.Visible = value && CanEverBeRolled;
            }
        }

        public bool CanEverBeRolled
        {
            get => _blnCanEverBeRolled;
            set
            {
                if (_blnCanEverBeRolled == value)
                    return;
                _blnCanEverBeRolled = value;
                cmdRoll.Visible = CanBeRolled && value;
            }
        }

        public int DicePool
        {
            get => _intDicePool;
            set
            {
                if (_intDicePool != value)
                {
                    _intDicePool = value;
                    lblDicePool.Text = CanBeRolled
                        ? _intDicePool.ToString(GlobalSettings.CultureInfo)
                        : _intDicePool.ToString("+#,0;-#,0;0", GlobalSettings.CultureInfo);
                }
            }
        }

        public string ToolTipText
        {
            get => lblDicePool.ToolTipText;
            set => lblDicePool.ToolTipText = value;
        }

        public ToolTip ToolTipObject => lblDicePool.ToolTipObject;
    }
}
