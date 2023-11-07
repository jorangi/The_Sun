using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject
{
    public List<Observer> obs = new();
    public void AddObserver(Observer observer)
    {
        if(!obs.Contains(observer)) obs.Add(observer);
    }
    public void RemoveObserver(Observer observer) 
    {
        if (obs.Contains(observer)) obs.Remove(observer);
    }
    public void NotifyObservers()
    {
        foreach(Observer observer in obs) observer.UpdateFromSubject();
    }
}
public interface Observer
{
    public void UpdateFromSubject();
}
