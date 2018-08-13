using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum ResourceType {None, Wood, Planks, Grain, Flour, Bread };
    public ResourceType type = ResourceType.None;
}
