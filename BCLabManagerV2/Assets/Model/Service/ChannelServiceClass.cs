using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ChannelServiceClass
    {
        public ObservableCollection<Channel> Items { get; set; }
        public void SuperAdd(Channel item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }
        public void DatabaseAdd(Channel item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.Tester = uow.Testers.GetById(item.Tester.Id);
                uow.Channels.Insert(item);
                uow.Commit();
            }
        }
        public void SuperRemove(int id)
        {
            DatabaseRemove(id);

            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public void DatabaseRemove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Channels.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(Channel item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(Channel item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Channels.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(Channel item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.Tester = item.Tester;
            edittarget.Name = item.Name;
            edittarget.AssetUseCount = item.AssetUseCount;
            edittarget.Records = item.Records;
        }
        public void Execute(Channel item, DateTime startTime, string programName, string recipeName)
        {
            item.AssetUseCount++;
            item.Records.Add(new AssetUsageRecord(startTime, item.AssetUseCount, programName, recipeName));

            SuperUpdate(item);
        }
        public void Commit(Channel item, DateTime endTime, string programName, string recipeName)
        {
            item.AssetUseCount--;
            item.Records.Add(new AssetUsageRecord(endTime, item.AssetUseCount, programName, recipeName));

            SuperUpdate(item);
        }
    }
}
