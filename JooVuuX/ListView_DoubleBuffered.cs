using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASUP
{
    public class ListView_DoubleBuffered : ListView
    {
        public ListView_DoubleBuffered() : base()
        {
            DoubleBuffered = true;
        }
    }
}
