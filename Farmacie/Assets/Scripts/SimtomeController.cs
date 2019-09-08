using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimtomeController : MonoBehaviour
{
    public Dropdown Dropdown;
    private List<Dropdown.OptionData> _dropdownList;

    public Transform SimtomsParent;

    private List<Simtom> _allSimtoms;

    private List<IPrefabComponent> _simtomsPool;

    // Start is called before the first frame update
    void Start()
    {
        RefreshOptions();
        Dropdown.onValueChanged.AddListener(delegate
        {
            OnSimtomChosen();
        });
    }

    public void RefreshOptions()
    {
        if (_allSimtoms == null)
            _allSimtoms = Data.Instance.GetSimtoms();

        _dropdownList = new List<Dropdown.OptionData>();
        _dropdownList.Add(new Dropdown.OptionData()
        {
            text = "Selecteaza un simptom..."
        });

        foreach (var simtom in _allSimtoms)
        {
            _dropdownList.Add(new Dropdown.OptionData()
            {
                text = simtom.nume
            });
        }

        if (Data.Instance.UserSimtoms != null)
        {
            foreach (var simtom in Data.Instance.UserSimtoms)
            {
                _dropdownList = _dropdownList.Where(l => l.text != simtom.nume).ToList();
            }
        }

        Dropdown.ClearOptions();
        Dropdown.AddOptions(_dropdownList);
    }

    public void OpenDropdown()
    {
        Dropdown.Show();
    }

    public void OnSimtomChosen()
    {
        if (Data.Instance.UserSimtoms == null)
            Data.Instance.UserSimtoms = new List<Simtom>();

        Data.Instance.UserSimtoms.Add(_allSimtoms.FirstOrDefault(s => s.nume == _dropdownList[Dropdown.value].text));

        RefreshOptions();
        ShowSimtoms();
    }

    public void ShowSimtoms()
    {
        if (Data.Instance.UserSimtoms == null || Data.Instance.UserSimtoms.Count == 0)
        {
            foreach (var simComp in _simtomsPool)
            {
                simComp.GameObject.SetActive(false);
            }
            return;
        }

        foreach (Simtom simtom in Data.Instance.UserSimtoms)
        {
            var wasNull = UsefullUtils.CheckInPool(
                simtom.id,
                GameHiddenOptions.Instance.SimtomPrefab,
                SimtomsParent,
                out IPrefabComponent simtomComponent,
                ref _simtomsPool
                );

            simtomComponent.Id = simtom.id;

            simtomComponent.GameObject.name = simtom.nume;
            (simtomComponent as SimtomComponent).Text.text = simtom.nume;

            //Sprite sprite = Resources.Load("ProductImages/" + product.PicturePath, typeof(Sprite)) as Sprite;
            //(simtomComponent as SimtomComponent).SetImage(sprite);

            if (wasNull)
            {
                _simtomsPool.Add(simtomComponent);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
