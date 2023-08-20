using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager<T> : NetworkManager where T : NetworkManager
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = singleton as T;
            }

            return instance;
        }
    }
}

public class NetworkRoomManager<T> : NetworkRoomManager where T : NetworkRoomManager
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = singleton as T;
            }

            return instance;
        }
    }
}

