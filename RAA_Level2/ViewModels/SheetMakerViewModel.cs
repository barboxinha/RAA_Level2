#region Namespaces
using RAA_Level2.Models;
using RAA_Level2.Utilities.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace RAA_Level2.ViewModels
{
    public class SheetMakerViewModel : ViewModelBase
    {
        private readonly SheetMakerModel _model;

        public SheetMakerModel Model => _model;


        public SheetMakerViewModel(SheetMakerModel model) 
        {
            _model = model;
        }
    }
}
