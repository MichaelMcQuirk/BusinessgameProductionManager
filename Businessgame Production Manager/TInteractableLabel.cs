using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class TInteractableLabel : Label
    {

        TInteractableLabel(String text, System.Drawing.Point location, System.Drawing.Font font)
        {
            Text = text;
            Location = location;
            AutoSize = true;
            Font = font;

        }
    }
}
