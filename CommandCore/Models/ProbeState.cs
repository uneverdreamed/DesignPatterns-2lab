using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCore.Models
{
    // состояние космического зонда
    public class ProbeState
    {
        public bool IsInSafeMode { get; set; } // зонд в безопасном режиме
        public int BatteryLevel { get; set; }
        public bool InstrumentsDeployed { get; set; } // подготовленные инструменты зонда
        public double CurrentSpeed { get; set; } // текущ. скорость в м/с

        public ProbeState()
        {
            IsInSafeMode = false;
            BatteryLevel = 100;
            InstrumentsDeployed = false;
            CurrentSpeed = 0;
        }

        public ProbeState(int batteryLevel, bool safeMode = false)
        {
            BatteryLevel = batteryLevel;
            IsInSafeMode = safeMode;
            InstrumentsDeployed = false;
            CurrentSpeed = 0;
        }
    }
}
