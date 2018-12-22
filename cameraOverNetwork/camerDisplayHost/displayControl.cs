using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace camerDisplayHost
{
    public class displayControl
    {
        public ImageBox _cibox { get; set; }
        public Control _ctrl { get; set; }
        public bool inUse { get; set; }
        public int id{ get; set; }

    }
}
