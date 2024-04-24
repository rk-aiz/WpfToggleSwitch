using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace WpfToggleSwitch
{
    public abstract class CustomButtonBase : ButtonBase
    {
        public int GridRow
        {
            set { Grid.SetRow(this, value); }
        }

        public int GridColumn
        {
            set { Grid.SetColumn(this, value); }
        }
    }
}
