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

using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chummer
{
    public interface IHasNotes
    {
        string Notes { get; set; }

        Color NotesColor { get; set; }

        Color PreferredColor { get; }
    }

    public static class Notes
    {
        /// <summary>
        /// Writes notes to an IHasNotes object, returns True if notes were changed and False otherwise.
        /// </summary>
        /// <param name="objNotes"></param>
        /// <param name="treNode"></param>
        public static async ValueTask<bool> WriteNotes(this IHasNotes objNotes, TreeNode treNode)
        {
            if (objNotes == null || treNode == null)
                return false;
            Form frmToUse = treNode.TreeView.FindForm() ?? Program.MainForm;

            DialogResult eResult = await frmToUse.DoThreadSafeFunc(async () =>
            {
                using (EditNotes frmItemNotes = new EditNotes(objNotes.Notes, objNotes.NotesColor))
                {
                    await frmItemNotes.ShowDialogSafeAsync(frmToUse);
                    if (frmItemNotes.DialogResult != DialogResult.OK)
                        return frmItemNotes.DialogResult;

                    objNotes.Notes = frmItemNotes.Notes;
                    objNotes.NotesColor = frmItemNotes.NotesColor;
                }

                treNode.ForeColor = objNotes.PreferredColor;
                treNode.ToolTipText = objNotes.Notes.WordWrap();

                return DialogResult.OK;
            });

            return eResult == DialogResult.OK;
        }
    }
}
