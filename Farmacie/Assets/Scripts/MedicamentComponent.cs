using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedicamentComponent : MonoBehaviour, IPrefabComponent
{
    public Image Image;
    public Text Name;

    private int _id;
    public int Id { get { return _id; } set { _id = value; } }

    public GameObject GameObject { get { return this.gameObject; } }

    public void SetImage(Sprite sprite)
    {
        Image.sprite = sprite;
    }
}
