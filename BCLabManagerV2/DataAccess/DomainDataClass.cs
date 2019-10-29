using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.DataAccess
{
    public class DomainDataClass
    {
        public ObservableCollection<BatteryTypeClass> BatteryTypes { get; set; }
        public ObservableCollection<BatteryClass> Batteries { get; set; }
        public ObservableCollection<TesterClass> Testers { get; set; }
        public ObservableCollection<ChannelClass> Channels { get; set; }
        public ObservableCollection<ChamberClass> Chambers { get; set; }
        public ObservableCollection<SubProgramTemplate> SubProgramTemplates { get; set; }
        public ObservableCollection<ChargeTemperatureClass> ChargeTemperatures { get; set; }
        public ObservableCollection<ChargeCurrentClass> ChargeCurrents { get; set; }
        public ObservableCollection<DischargeTemperatureClass> DischargeTemperatures { get; set; }
        public ObservableCollection<DischargeCurrentClass> DischargeCurrents { get; set; }
        public ObservableCollection<ProgramClass> Programs { get; set; }
    }
}
