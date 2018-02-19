using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


namespace GameTimeController
{
    public class GameTimeController : MonoBehaviour
    {
        UnityEvent timeEvent = new UnityEvent();
        public enum TimeEnum { none, oneTenth, halfSecond, second }
        List<GameTimeLayer> listLayer = new List<GameTimeLayer>();

        public static bool isTime = false;
        float timeCount = 0;


        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            StepTime();
        }

        public void InitTime(GameTimeData data)
        {
            gameObject.name = data.name;
            CreateLayer(TimeEnum.oneTenth, 0.1f);
            CreateLayer(TimeEnum.halfSecond, 0.5f);
            CreateLayer(TimeEnum.second, 1);
        }

        public void AddEvent(UnityAction unityAction, TimeEnum _timeEnum)
        {
            for (int i = 0; i < listLayer.Count; i++)
            {
                if (listLayer[i].timeEnum == _timeEnum)
                {
                    listLayer[i].timeEvent.AddListener(unityAction);
                }
            }
        }

        public void RemoveEvent(UnityAction unityAction, TimeEnum _timeEnum)
        {
            for (int i = 0; i < listLayer.Count; i++)
            {
                if (listLayer[i].timeEnum == _timeEnum)
                {
                    listLayer[i].timeEvent.RemoveListener(unityAction);
                }
            }
        }

        void CreateLayer(TimeEnum _timeEnum, float _time)
        {
            GameTimeLayer gameTimeLayer = new GameTimeLayer();
            gameTimeLayer.maxTime = _time;
            gameTimeLayer.timeEnum = _timeEnum;
            listLayer.Add(gameTimeLayer);
        }



        void StepTime()
        {
            for (int i = 0; i < listLayer.Count; i++)
            {
                listLayer[i].UpdateTime();
            }
        }

    }

    public class GameTimeData
    {
        public string name;
    }

    public class GameTimeLayer
    {
        public GameTimeController.TimeEnum timeEnum;
        public float maxTime;
        float nowTime = 0;
        public UnityEvent timeEvent = new UnityEvent();

        public void UpdateTime()
        {
            nowTime += Time.deltaTime;
            if (nowTime >= maxTime)
            {
                nowTime = 0;
                timeEvent.Invoke();
            }
        }
    }
}
