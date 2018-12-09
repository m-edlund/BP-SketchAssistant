using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchAssistant
{
    class FileImporterException : Exception
    {
        /// <summary>
        /// the clean and formatted message to show to the user
        /// </summary>
        String showMessage;

        public FileImporterException(String message, String hint, int lineNumber) : base (message)
        {
            showMessage = "Could not import file:\n\n" + message + (hint == null ? "" : "\n(Hint: " + hint + ")") + (lineNumber == -1 ? "" : "\n\n-line: " + lineNumber );
        }

        public override string ToString()
        {
            return showMessage;
        }
    }
}
