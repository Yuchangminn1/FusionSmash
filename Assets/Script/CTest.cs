using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
public class CTest : MonoBehaviour
{

    delegate void EventHandler(string message);

    class myNotifier
    {
        public event EventHandler SomethingHappened;
        public void DoSomething(int number)
        {
            int temp = number % 10;
            if(temp != 0 && temp %3 == 0)
            {
                SomethingHappened($"{number} : ¦");

            }
        }
    }

    static public void MyHandler(string message)
    {
        Debug.Log(message);
    }


    static void FF()
    {
        myNotifier notifier = new myNotifier();
        notifier.SomethingHappened += MyHandler;

        for (int i = 0; i < 30; i++) 
        {
            notifier.DoSomething(i);
        }
    }

    private void Start()
    {
        FF();
    }


}
