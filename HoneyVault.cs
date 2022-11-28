using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeHive
{
    static class HoneyVault
    {
        public const float NECTAR_CONVERSION_RATIO = .19f;
        public const float LOW_LEVEL_WARNING = 10f;
        private static float honey = 25f;
        private static float nectar = 100f;

        public static void CollectNectar(float amount)
        {
            if (amount > 0f) nectar += amount;
        }

        public static void ConvertNectarToHoney(float amount)
        {
            float nectarToConvert = amount;
            if (nectarToConvert > nectar) nectarToConvert = nectar;
            nectar -= nectarToConvert;
            honey += nectarToConvert * NECTAR_CONVERSION_RATIO;
        }

        public static bool ConsumeHoney(float amount)
        {
            if (honey >= amount)
            {
                honey -= amount;
                return true;
            }
            return false;
        }

        public static string StatusReport
        {
            get
            {
                string status = $"{honey:0.0} единицы меда\n" +
                                $"{nectar:0.0} единицы нектара";
                string warnings = "";
                if (honey < LOW_LEVEL_WARNING) warnings +=
                                    "\nМАЛО МЕДА - ДОБАВИТЬ ПРОИЗВОДИТЕЛЯ МЕДА";
                if (nectar < LOW_LEVEL_WARNING) warnings +=
                                    "\nМАЛО НЕКТАРА - ДОБАВИТЬ СОБИРАТЕЛЯ НЕКТАРА";
                return status + warnings;
            }
        }
    }
}
