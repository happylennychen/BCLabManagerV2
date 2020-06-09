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
        public ObservableCollection<BatteryType> BatteryTypes { get; set; }
        public ObservableCollection<Battery> Batteries { get; set; }
        public ObservableCollection<Tester> Testers { get; set; }
        public ObservableCollection<Channel> Channels { get; set; }
        public ObservableCollection<Chamber> Chambers { get; set; }
        public ObservableCollection<RecipeTemplate> RecipeTemplates { get; set; }
        //public ObservableCollection<ChargeTemperatureClass> ChargeTemperatures { get; set; }
        //public ObservableCollection<ChargeCurrentClass> ChargeCurrents { get; set; }
        //public ObservableCollection<DischargeTemperatureClass> DischargeTemperatures { get; set; }
        //public ObservableCollection<DischargeCurrentClass> DischargeCurrents { get; set; }
        public ObservableCollection<Program> Programs { get; set; }
        public DomainDataClass()
        {
            LoadFromDB();
            EventBooking();
        }
        private void LoadFromDB()
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                BatteryTypes = new ObservableCollection<BatteryType>(uow.BatteryTypes.GetAll());
                Batteries = new ObservableCollection<Battery>(uow.Batteries.GetAll("BatteryType,Records"));
                Testers = new ObservableCollection<Tester>(uow.Testers.GetAll());
            }
        }

        private void EventBooking()
        {
            BatteryTypes.CollectionChanged += BatteryTypes_CollectionChanged;
            foreach (var batteryType in BatteryTypes)
            {
                batteryType.PropertyChanged += BatteryType_PropertyChanged;
            }

            Batteries.CollectionChanged += Batteries_CollectionChanged;
            foreach(var battery in Batteries)
            {
                battery.PropertyChanged += Battery_PropertyChanged;
            }
        }

        private void Battery_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Battery battery = sender as Battery;
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Batteries.Update(battery);
                uow.Commit();
            }
        }

        private void Batteries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach(var item in e.NewItems)
                    {
                        var battery = item as Battery;
                        battery.PropertyChanged += Battery_PropertyChanged;
                        using(var uow = new UnitOfWork(new AppDbContext()))
                        {
                            uow.Batteries.Insert(battery);
                            uow.Commit();
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach(var item in e.OldItems)
                    {
                        var battery = item as Battery;
                        using(var uow = new UnitOfWork(new AppDbContext()))
                        {
                            uow.Batteries.Delete(battery);
                            uow.Commit();
                        }
                    }
                    break;
            }
        }

        private void BatteryType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            BatteryType batteryType = sender as BatteryType;
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
                        var batteryType = item as BatteryType;
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
                        var batteryType = item as BatteryType;
                        using (var uow = new UnitOfWork(new AppDbContext()))
                        {
                            uow.BatteryTypes.Delete(batteryType);
                            uow.Commit();
                        }
                    }
                    break;
            }
        }

    }
}
