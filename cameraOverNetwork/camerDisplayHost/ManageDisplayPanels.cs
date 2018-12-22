using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace camerDisplayHost
{
    public class ManageDisplayControls
    {
        public List<displayControl> _displayCtrl = new List<displayControl>();
        private int nCounter = 0;

        

        public ManageDisplayControls( )
        {
            
        }

        public void InitializeDispCtrls( object pan, object cib)
        {
            displayControl dc = new displayControl();
            dc._ctrl = (Panel)pan;
            dc._cibox = (ImageBox)cib;
            dc.inUse = false;
            

            ////
            ((System.ComponentModel.ISupportInitialize)(dc._cibox)).BeginInit();
            dc._ctrl.Controls.Add((ImageBox)cib);
            dc._cibox.Dock = System.Windows.Forms.DockStyle.Fill;
            dc._cibox.Location = new System.Drawing.Point(0, 0);
            dc._cibox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            dc._cibox.Name = "captureImageBox"+(++nCounter);
            dc._cibox.Size = new System.Drawing.Size(298, 202);
            dc._cibox.TabIndex = 1;
            dc._cibox.TabStop = false;
            ((System.ComponentModel.ISupportInitialize)(dc._cibox)).EndInit();

            dc.id = _displayCtrl.Count +1;

            _displayCtrl.Add(dc);
        }

        public object getAvailableDisplayNotInUse( )
        {
            return _displayCtrl.Find(x => x.inUse == false);
        }

        public void setDisplayIndexInUSe(object cibox)
        {
            _displayCtrl.Find(x => x._cibox == (ImageBox) cibox).inUse = true;
        }

        public void ResetDisplayIndexInUSe(object cibox)
        {
            _displayCtrl.Find(x => x._cibox == (ImageBox)cibox).inUse = false;
        }

    }
}
