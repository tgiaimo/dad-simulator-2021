using UnityEngine;
using System.Collections;

namespace RASCAL {
    public static class RascalExtensions {


        public static void Lap(this System.Diagnostics.Stopwatch sw, string msg = "") {
            sw.Stop();
            Debug.Log(msg + "-" + sw.ElapsedMilliseconds + ":" + sw.ElapsedTicks);
            sw.Restart();
        }

        public static double LapTime(this System.Diagnostics.Stopwatch sw) {
            sw.Stop();
            var time = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            return time;
        }

        public static void Restart(this System.Diagnostics.Stopwatch sw) {
            sw.Reset();
            sw.Start();
        }

    }


#if NET_4_6 || CSHARP_7_3_OR_NEWER
    public class TaskYield : CustomYieldInstruction {

        System.Threading.Tasks.Task task;

        public TaskYield(System.Threading.Tasks.Task taskToYield) {
            task = taskToYield;
        }

        public override bool keepWaiting { get { return !task.IsCompleted; } }

    }
#endif

}
