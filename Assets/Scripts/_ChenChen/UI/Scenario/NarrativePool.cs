using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ChenChen_UI
{
    public class NarrativePool
    {
        private static readonly string _narrativeLogPath = "UI/Component/NarrativeLog";

        private List<NarrativeLog> _inactivePool;

        private List<NarrativeLog> _activePool;

        private GameObject _parent;

        private int _poolMaxSize;

        private int _countAll;

        private int _countActive => _activePool.Count;

        private int _countInactive => _inactivePool.Count;


        public NarrativePool(GameObject parent, int poolMaxSize)
        {
            _inactivePool = new List<NarrativeLog>();
            _activePool = new List<NarrativeLog>();
            _parent = parent;
            _poolMaxSize = poolMaxSize;
        }

        private NarrativeLog Create()
        {
            NarrativeLog narrative = Object.Instantiate(Resources.Load(_narrativeLogPath) as GameObject, _parent.transform).GetComponent<NarrativeLog>();
            narrative.gameObject.SetActive(false);
            _countAll++;
            //Debug.Log($"新建第 {_countAll} 个对象" + narrative.name);
            return narrative;
        }
        public NarrativeLog Get()
        {         
            NarrativeLog result;
            if(_countActive == _poolMaxSize)
            {
                result = _activePool[0];
                _activePool.RemoveAt(0);
                //Debug.Log(result.name + "重新进池");
            }
            else
            {
                if (_countInactive > 0)
                {
                    int count = _countInactive - 1;
                    result = _inactivePool[count];
                    _inactivePool.RemoveAt(count);
                    //Debug.Log(result.name + "出池");
                }
                else
                {
                    result = Create();
                    //Debug.Log(result.name + "新建");
                }
            }
            result.gameObject.SetActive(true);
            result.gameObject.transform.SetSiblingIndex(0);
            _activePool.Add(result);

            return result;
        }
        public void Release(NarrativeLog narrative)
        {
            narrative.gameObject.SetActive(false);//隐藏
            _activePool.Remove(narrative);
            _inactivePool.Add(narrative); 
        }
        public void Clear()
        {
            foreach(var narrative in _activePool)
            {
                _inactivePool.Add(narrative);
                narrative.gameObject?.SetActive(false);
            }
            _activePool.Clear();
        }
    }
}
