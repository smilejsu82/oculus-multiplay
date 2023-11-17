using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 public delegate void EventDispatcherDelegate(object evtData);

    public interface IEventDispatcher
    {
        void AddListener(string evtName, EventDispatcherDelegate callback);
        void RemoveListener(string evtName, EventDispatcherDelegate callback);
        void Dispatch(string evtName, object evt);

    }
    public sealed class EventDispatcher : IEventDispatcher
    {
        public static readonly EventDispatcher instance = new EventDispatcher();

        //생성자 
        private EventDispatcher(){ }

        Dictionary<string, List<EventDispatcherDelegate>> listeners = new Dictionary<string, List<EventDispatcherDelegate>>();

        public void AddListener(string evtName, EventDispatcherDelegate callback)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(evtName, out evtListeners))
            {
                evtListeners.Remove(callback); //make sure we dont add duplicate
                evtListeners.Add(callback);
            }
            else
            {
                evtListeners = new List<EventDispatcherDelegate>();
                evtListeners.Add(callback);

                listeners.Add(evtName, evtListeners);
            }
        }
        public void RemoveListener(string evtName, EventDispatcherDelegate callback)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(evtName, out evtListeners))
            {
                for (int i = 0; i < evtListeners.Count; i++)
                {
                    evtListeners.Remove(callback);
                }
            }
        }
        public void Dispatch(string evtName, object evt)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(evtName, out evtListeners))
            {
                for (int i = 0; i < evtListeners.Count; i++)
                {
                    evtListeners[i](evt);
                }
            }
        }
    }