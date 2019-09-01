using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPrefabComponent
{
    int Id { get; set; }
    //Route Route { get; set; }

    GameObject GameObject { get; }
}