using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class GlobalSettings
    {
        private static bool _isGraphModeAdvancedByDefault = false;

        public GlobalSettings()
        {
           
        }

        public static bool IsGraphModeAdvancedByDefault {
            get {
                return _isGraphModeAdvancedByDefault;
            }
            set
            {
                _isGraphModeAdvancedByDefault = value;
            }
        }
    }
}
