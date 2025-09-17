using System.Collections.Generic;
using UnityEngine;

namespace FinnTeichler.Interface
{
    public static class InterfaceUtility
    {
        public static void GetInterfaces<T>(out List<T> resultList, out bool isEmpty, GameObject objectToSearch)
            where T : class
        {
            MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
            resultList = new List<T>();
            isEmpty = true;

            foreach (MonoBehaviour mb in list)
            {
                if (mb is T)
                {
                    resultList.Add((T)((System.Object)mb));
                    isEmpty = false;
                }
            }
        }

        public static void GetInterface<T>(out T result, out bool isNull, GameObject objectToSearch) where T : class
        {
            MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
            result = null;
            isNull = true;

            foreach (MonoBehaviour mb in list)
            {
                if (mb is T)
                {
                    result = (T)(System.Object)mb;
                    isNull = false;
                }
            }
        }
    }
}