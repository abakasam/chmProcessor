/* 
 * chmProcessor - Word converter to CHM
 * Copyright (C) 2008 Toni Bennasar Obrador
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Tidy;

namespace ChmProcessorLib
{
    /// <summary>
    /// Class to clean and repair HTML with Tidy (http://tidy.sourceforge.net/)
    /// </summary>
    public class TidyParser
    {
        private UserInterface ui;
        private bool XmlOutput;

        /// <summary>
        /// Encoding for input files.
        /// If null, we will use the default (tidy documentation say latin1)
        /// </summary>
        public string InputEncoding = null;

        /// <summary>
        /// Encoding for output files.
        /// If null, we will use the default (tidy documentation say ascii)
        /// </summary>
        public string OutputEncoding = null;

        public TidyParser(UserInterface ui)
        {
            this.ui = ui;
            this.XmlOutput = false;
        }

        public TidyParser(UserInterface ui, bool xmlOutput)
        {
            this.ui = ui;
            this.XmlOutput = xmlOutput;
        }

        /// <summary>
        /// Configures tidy to make the conversion / repair.
        /// </summary>
        /// <returns>The document to make the conversion</returns>
        protected Document ConfigureParse()
        {
            Document tdoc = new Document();
            int status = 0;
            // Set alternative text for IMG tags:
            status = tdoc.SetOptValue(TidyOptionId.TidyAltText, "image");
            CheckStatus(status);

            if (XmlOutput)
                status = tdoc.SetOptBool(TidyOptionId.TidyXhtmlOut, 1);
            CheckStatus(status);

            if(InputEncoding != null)
                status = tdoc.SetOptValue(TidyOptionId.TidyInCharEncoding, InputEncoding);
            CheckStatus(status);

            if (OutputEncoding != null)
                status = tdoc.SetOptValue(TidyOptionId.TidyOutCharEncoding, OutputEncoding);
            CheckStatus(status);

            // Modify the original file. Not working??
            //tdoc.SetOptValue(TidyOptionId.TidyWriteBack, "yes");
            //CheckStatus(status);

            return tdoc;
        }

        public void Parse( string file ) {

            try
            {
                log("Parsing file " + file + "...", 2);

                Document tdoc = ConfigureParse();

                int status = 0;
                status = tdoc.ParseFile(file);
                CheckStatus(status);

                status = tdoc.CleanAndRepair();
                CheckStatus(status);

                status = tdoc.SaveFile(file);
                CheckStatus(status);
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }

        public string ParseString(string htmlText)
        {
            log("Parsing html...", 2);

            Document tdoc = ConfigureParse();

            int status = 0;
            status = tdoc.ParseString(htmlText);
            CheckStatus(status);

            status = tdoc.CleanAndRepair();
            CheckStatus(status);

            string cleanHtml = tdoc.SaveString();
            CheckStatus(status);

            return cleanHtml;
        }

        private void CheckStatus(int status) {
            if (status < 0)
                throw new Exception("Error runing Tidy.NET: " + status);
        }

        private void log(string text, int level)
        {
            if (ui != null)
                ui.log("Tidy: " + text, level);
        }

        private void log(Exception ex)
        {
            if (ui != null)
                ui.log(ex);
        }

    }
}
