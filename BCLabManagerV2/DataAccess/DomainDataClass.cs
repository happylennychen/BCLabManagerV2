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
        public DomainDataClass()
        {
            LoadFromDB();
            BatteryTypes.CollectionChanged += BatteryTypes_CollectionChanged;
            foreach (var batteryType in BatteryTypes)
            {
                batteryType.PropertyChanged += BatteryType_PropertyChanged;
            }
        }

        private void BatteryType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            BatteryTypeClass batteryType = sender as BatteryTypeClass;
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.BatteryTypes.Update(batteryType);
                uow.Commit();
            }
        }

        private void BatteryTypes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var batteryType = item as BatteryTypeClass;
                        batteryType.PropertyChanged += BatteryType_PropertyChanged;
                        using (var uow = new UnitOfWork(new AppDbContext()))
                        {
                            uow.BatteryTypes.Insert(batteryType);
                            uow.Commit();
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var batteryType = item as BatteryTypeClass;
                        using (var uow = new UnitOfWork(new AppDbContext()))
                        {
                            uow.BatteryTypes.Delete(batteryType);
                            uow.Commit();
                        }
                    }
                    break;
            }
        }

        public void LoadFromDB()
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                BatteryTypes = new ObservableCollection<BatteryTypeClass>(uow.BatteryTypes.GetAll());
                Batteries = new ObservableCollection<BatteryClass>(uow.Batteries.GetAll("BatteryType,Records"));
                Testers = new ObservableCollection<TesterClass>(uow.Testers.GetAll());
            }
        }
    }
}
