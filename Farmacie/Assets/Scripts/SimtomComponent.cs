using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimtomComponent : MonoBehaviour, IPrefabComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }

    public GameObject GameObject { get { return this.gameObject; } }

    public Text Text;
}
