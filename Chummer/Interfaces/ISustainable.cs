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

using Chummer.Backend.Skills;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using NLog;

namespace Chummer
{
    public interface ISustainable
    {
        /// <summary>
        /// Force of the spell or complex form
        /// </summary>
        int Force { get; set; }

        /// <summary>
        /// Net Hits of the casting test
        /// </summary>
        int NetHits { get; set; }

        /// <summary>
        /// Is the object sustained by the character?
        /// </summary>
        bool SelfSustained { get; set; }

        /// <summary>
        /// Load an Sustainable Object from an given XML node. (Defaut Path should be sustained/sustainedobject)
        /// </summary>
        /// <param name="objNode"></param>
        void Load(XmlNode objNode);

        /// <summary>
        /// Print the object's XML to the XmlWriter.
        /// </summary>
        /// <param name="objWriter"></param>
        /// <param name="objCulture"></param>
        /// <param name="strLanguageToPrint"></param>
        void Print(XmlTextWriter objWriter, CultureInfo objCulture, string strLanguageToPrint);

        /// <summary>
        /// Saves an Sustainable object to .xml
        /// </summary>
        /// <param name="objWriter"></param>
        void Save(XmlTextWriter objWriter);

        /// <summary>
        /// Returns the Display name of an given Spell or Complex Form
        /// </summary>
        /// <param name="strLanguage"></param>
        /// <returns></returns>
        string DisplayName(string strLanguage);
    }
}
