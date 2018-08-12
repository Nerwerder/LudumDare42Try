using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Buildings.BuildingType type;
    protected Place place;


    private void Update()
    {
        Work(Time.deltaTime);
    }

    public void SetPlace(Place p)
    {
        place = p;
    }

    public virtual void Init()
    {

    }

    public virtual void Work(float time)
    {

    }

    public virtual void Done()
    {

    }
}
