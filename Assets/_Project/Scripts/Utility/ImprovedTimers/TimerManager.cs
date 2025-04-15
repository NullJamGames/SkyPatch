using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace NJG.Utilities.ImprovedTimers
{
    public static class TimerManager 
    {
        private static readonly List<Timer> _timers = new();

        public static void RegisterTimer(Timer timer) => _timers.Add(timer);
        public static void DeregisterTimer(Timer timer) => _timers.Remove(timer);

        private static void UpdateTimers() 
        {
            foreach (Timer timer in new List<Timer>(_timers)) 
            {
                timer.Tick(Time.deltaTime);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InsertTimerSystemIntoPlayerLoop() 
        {
            PlayerLoopSystem playerLoop = PlayerLoop.GetDefaultPlayerLoop();

            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type != typeof(Update)) continue;
                
                PlayerLoopSystem[] updateSubsystems = playerLoop.subSystemList[i].subSystemList;
                List<PlayerLoopSystem> newSubsystemList = new (updateSubsystems);

                PlayerLoopSystem timerLoopSystem = new()
                {
                    type = typeof(TimerManager),
                    updateDelegate = TimerManager.UpdateTimers
                };

                newSubsystemList.Insert(0, timerLoopSystem);
                playerLoop.subSystemList[i].subSystemList = newSubsystemList.ToArray();
                break;
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
        }
    }
}