using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeeHive
{
    abstract class Bee //в учебнике без abstract
    {
        //Конструктор Bee получает 1 параметр, который используется для инициализации его свойства Job,
        //доступного только для чтения. Класс Queen использет это свойство при построении отчета, чтобы
        //определить, к какой разновидности относится эта конкретная пчела.
        public string Job { get; private set; }
        public Bee (string job)
        {
            Job = job;
        }

        //Виртуальной доступное только для чтение свойство CostPerShift, позволяет каждому субклассу Bee
        //определить кол-во меда, потребляемое им за смену
        public abstract float CostPerShift { get; } //в учебнике virtual 

        public void WorkTheNextShift ()
        {
            if (HoneyVault.ConsumeHoney(CostPerShift))
            {
                DoJob();
            }
        }
        protected abstract void DoJob();
    }

    class HoneyManufacturer : Bee 
    {
        public const float NECTAR_PROCESSRD_PER_SHIFT = 33.15f;
        public override float CostPerShift { get { return 1.7f; } }//Значение свойства Bee.CostPerShift, доступного только для чтения
        public HoneyManufacturer():base ("Производитель меда") { }
        protected override void DoJob () 
        {
            HoneyVault.ConvertNectarToHoney(NECTAR_PROCESSRD_PER_SHIFT);
        }
    }
    class NectarCollector : Bee 
    {
        public const float NECTAR_COLLECTED_PER_SHIFT = 33.25f;
        public override float CostPerShift { get { return 1.95f; } }//Значение свойства Bee.CostPerShift, доступного только для чтения
        public NectarCollector():base ("Сборщик нектара") { }
        protected override void DoJob () 
        {
            HoneyVault.CollectNectar(NECTAR_COLLECTED_PER_SHIFT);
        }
    }
    class EggCare : Bee
    {
        public const float CARE_PROGRESS_PER_SHIFT = 0.15f;
        public override float CostPerShift { get { return 1.35f; } }

        private Queen queen;

        public EggCare(Queen queen) : base("Уход за яйцом")
        {
            this.queen = queen;
        }

        protected override void DoJob()
        {
            queen.CareForEggs(CARE_PROGRESS_PER_SHIFT);
        }
    }
    class Queen : Bee
    {
        public const float EGGS_PER_SHIFT = 0.45f;
        public const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;

        private Bee[] workers = new Bee[0];
        private float eggs = 0;
        private float unassignedWorkers = 3;

        public string StatusReport { get; private set; }
        public override float CostPerShift { get { return 2.15f; } }

        public Queen() : base("Queen")
        {
            AssignBee("Сборщик нектара");
            AssignBee("Производитель меда");
            AssignBee("Уход за яйцом");
        }

        private void AddWorker(Bee worker)
        {
            if (unassignedWorkers >= 1)
            {
                unassignedWorkers--;
                Array.Resize(ref workers, workers.Length + 1);
                workers[workers.Length - 1] = worker;
            }
        }

        private void UpdateStatusReport()
        {
            StatusReport = $"Отчет о хранилище:\n{HoneyVault.StatusReport}\n" +
            $"\nКоличество яиц: {eggs:0.0}\nНеназначенные работники: {unassignedWorkers:0.0}\n" +
            $"{WorkerStatus("Сборщик нектара")}\n{WorkerStatus("Производитель меда")}" +
            $"\n{WorkerStatus("Уход за яйцом")}\n\r\nВСЕГО РАБОТНИКОВ: {workers.Length}";
        }

        public void CareForEggs(float eggsToConvert)
        {
            if (eggs >= eggsToConvert)
            {
                eggs -= eggsToConvert;
                unassignedWorkers += eggsToConvert;
            }
        }
        private string WorkerStatus(string job)
        {
            int count = 0;
            foreach (Bee worker in workers)
                if (worker.Job == job) count++;
            string s = "а";
            if (count != 1) s = "";
            return $"{count} {job} пчел{s}";
        }

        public void AssignBee(string job)
        {
            switch (job)
            {
                case "Сборщик нектара":
                    AddWorker(new NectarCollector());
                    break;
                case "Производитель меда":
                    AddWorker(new HoneyManufacturer());
                    break;
                case "Уход за яйцом":
                    AddWorker(new EggCare(this));
                    break;
            }
            UpdateStatusReport();
        }

        protected override void DoJob()
        {
            eggs += EGGS_PER_SHIFT;
            foreach (Bee worker in workers)
            {
                worker.WorkTheNextShift();
            }
            HoneyVault.ConsumeHoney(unassignedWorkers * HONEY_PER_UNASSIGNED_WORKER);
            UpdateStatusReport();
        }
    }
}
