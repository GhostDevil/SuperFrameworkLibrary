using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFramework.AppLogs
{
    public class MyLayout : PatternLayout
    {
        public MyLayout()
        {

            AddConverter("Property", typeof(MyPatternConverter));
        }
    }
}
