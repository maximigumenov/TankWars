using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;



public static class GameTime
{
    static string nameController = "GameTimeController";
    static GameTimeController.GameTimeController controller;

    static GameTime()
    {
        if (!GameTimeController.GameTimeController.isTime)
        {
            StartTime();
        }
    }

    public static void AddEvent(UnityAction unityAction, GameTimeController.GameTimeController.TimeEnum _timeEnum)
    {
        controller.AddEvent(unityAction, _timeEnum);
    }

    public static void RemoveEvent(UnityAction unityAction, GameTimeController.GameTimeController.TimeEnum _timeEnum)
    {
        controller.RemoveEvent(unityAction, _timeEnum);
    }

    static void StartTime()
    {

        GameObject controllerObject = new GameObject();
        controllerObject.AddComponent<GameTimeController.GameTimeController>();
        controller = controllerObject.GetComponent<GameTimeController.GameTimeController>();
        GameTimeController.GameTimeData data = new GameTimeController.GameTimeData();
        data.name = nameController;
        controller.InitTime(data);
    }
}

