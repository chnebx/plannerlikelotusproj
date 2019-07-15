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
        private static string _DefaultDBFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "NarnyaDB");


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

        public static string DefaultDBFolder
        {
            get
            {
                return _DefaultDBFolder;
            }
            set
            {
                _DefaultDBFolder = value;
            }
        }
    }
}
