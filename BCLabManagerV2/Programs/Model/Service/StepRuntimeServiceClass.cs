using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class StepRuntimeServiceClass
    {
        public ObservableCollection<StepRuntimeClass> Items { get; set; }
        //public StepServiceClass StepService { get; set; } = new StepServiceClass();
        //public StepTemplateServiceClass StepTemplateService { get; set; } = new StepTemplateServiceClass();
        //public void Add(StepRuntimeClass item)
        //{
        //using (var uow = new UnitOfWork(new AppDbContext()))
        //{
        //    item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
        //    uow.Batteries.Insert(item);
        //    uow.Commit();
        //}
        //Items.Add(item);
        //}
        //public void Remove(int id)
        //{
        //using (var uow = new UnitOfWork(new AppDbContext()))
        //{
        //    uow.Batteries.Delete(id);
        //    uow.Commit();
        //}

        //var item = Items.SingleOrDefault(o => o.Id == id);
        //Items.Remove(item);
        //}
        public void Update(StepRuntimeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.StepRuntimes.Update(item);
                uow.Commit();
            }
            //var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            //edittarget.EndTime = item.EndTime;
            //edittarget.IsAbandoned = item.IsAbandoned;
            //edittarget.Loop = item.Loop;
            //edittarget.StartTime = item.StartTime;
            //edittarget.TestRecords = item.TestRecords;
            //edittarget.Name = item.Name;
        }
        internal TimeSpan GetDuration(StepRuntimeClass sr, ref double CBegin)       //计算Duration，并且为下一个sr更新CBegin
        {
            double Cend = 0;
            TimeSpan duration;
            var st = sr.StepTemplate;
            if (st.CutOffConditionType == CutOffConditionTypeEnum.Time_s)
            {
                duration = TimeSpan.FromSeconds(st.CutOffConditionValue);
                Cend = CBegin + sr.GetCurrentInmA() * duration.TotalHours;
            }
            else
            {
                if (st.CutOffConditionType == CutOffConditionTypeEnum.CRate)
                    Cend = st.CutOffConditionValue * sr.DesignCapacityInmAH;
                else if (st.CutOffConditionType == CutOffConditionTypeEnum.C_mAH)
                    Cend = st.CutOffConditionValue;

                duration = TimeSpan.FromHours(GetTimeInSecondsWithParameters(Cend, CBegin, sr.GetCurrentInmA(), sr.StepTemplate.Coefficient.Slope, sr.StepTemplate.Coefficient.Offset));
            }
            CBegin = Cend;
            return duration;

        }
        private double GetTimeInSecondsWithParameters(double Cend, double CBegin, double Current, double Slope, double Offset)
        {
            return Slope * GetTimeInSeconds(Cend, CBegin, Current) + Offset;
        }
        private double GetTimeInSeconds(double Cend, double CBegin, double Current)
        {
            return (Cend - CBegin) / Current;
        }

        internal void UpdateEstimatedTime(StepRuntimeClass item, StepRuntimeClass startPoint, ref DateTime time, ref double c, ref bool isActive)
        {
            if (isActive)
            {
                UpdateEstimatedTime(item, ref time, ref c);
            }
            else
            {
                if (item == startPoint)
                {
                    isActive = true;
                    UpdateEstimatedTime(item, ref time, ref c);
                }
            }
        }

        internal void UpdateEstimatedTime(StepRuntimeClass item, ref DateTime time, ref double c)
        {
            if (item.StartTime == DateTime.MinValue)
            {
                item.EST = time;
                time += GetDuration(item, ref c);
                item.EET = time;
            }
            else
            {
                time = item.StartTime;
                if (item.EndTime == DateTime.MinValue)
                {
                    time += GetDuration(item, ref c);
                    item.EET = time;
                }
                else
                {
                    GetDuration(item, ref c);
                    time = item.EndTime;
                }
            }
        }
    }
}
