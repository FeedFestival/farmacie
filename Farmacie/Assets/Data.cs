using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    private static Data _data;
    public static Data Instance
    {
        get
        {
            return _data;
        }
    }

    public string TraingDataPath = "farmacie_training-data.csv";
    public string TestDataPath = "farmacie_test-data.csv";

    public string TraingDangerousDataPath = "farmacie_dangerous_training-data.csv";
    public string TestDangerousDataPath = "farmacie_dangerous_test-data.csv";

    public List<Simtom> UserSimtoms;

    private List<Simtom> _simtoms;
    private List<Medicament> _medicamente;

    // Start is called before the first frame update
    void Awake()
    {
        _data = this;
    }

    public List<Simtom> GetSimtoms()
    {
        if (_simtoms == null)
        {
            _simtoms = new List<Simtom>()
            {
                new Simtom(){ id = 1, nume = "Durere de cap" },
                new Simtom(){ id = 2, nume = "Dureri de gât" },
                new Simtom(){ id = 3, nume = "Tuse" },
                new Simtom(){ id = 4, nume = "Nas înfundat" },
                new Simtom(){ id = 5, nume = "Febră" },
                new Simtom(){ id = 6, nume = "Dureri musculare" },
                new Simtom(){ id = 7, nume = "Frisoane" },
                new Simtom(){ id = 8, nume = "Constipație" },
                new Simtom(){ id = 9, nume = "Diaree" },
                new Simtom(){ id = 10, nume = "Insomnii" },
            };
        }

        return _simtoms;
    }

    public List<Medicament> GetMedicamente()
    {
        if (_medicamente == null)
        {
            _medicamente = new List<Medicament>()
            {
                new Medicament(){ id = 1, nume = "Brufen 400", pic = "Brufen" },
                new Medicament(){ id = 2, nume = "Nurofen Express Forte", pic = "Nurofen" },
                new Medicament(){ id = 3, nume = "Fasconal", pic = "Fasconal" },
                new Medicament(){ id = 4, nume = "Solpadeine", pic = "Solpadeine" },
                new Medicament(){ id = 5, nume = "Ibusinus", pic = "Ibusinus" },
                new Medicament(){ id = 6, nume = "Coldrex MaxGrip", pic = "Coldrex" },
                new Medicament(){ id = 7, nume = "Strepsils", pic = "Strepsils" },
                new Medicament(){ id = 8, nume = "ACC", pic = "ACC" },
                new Medicament(){ id = 9, nume = "Tusocalm", pic = "Tusocalm" },
                new Medicament(){ id = 10, nume = "Bixtonim", pic = "Bixtonim" },
                new Medicament(){ id = 11, nume = "Vibrocil", pic = "Vibrocil" },
                new Medicament(){ id = 12, nume = "Imodium", pic = "Imodium" },
                new Medicament(){ id = 13, nume = "Theraflu", pic = "Theraflu" },
                new Medicament(){ id = 14, nume = "Smecta", pic = "Smecta" },
                new Medicament(){ id = 15, nume = "Meltus sirop", pic = "Meltus" },
                new Medicament(){ id = 16, nume = "Aspirină", pic = "Aspirina" },
                new Medicament(){ id = 17, nume = "Hexoral Spray", pic = "Hexoral" },
                new Medicament(){ id = 18, nume = "Sennalax", pic = "Sennalax" },
                new Medicament(){ id = 19, nume = "Melatonină", pic = "Melatonina" },
                new Medicament(){ id = 20, nume = "Calmogen Plant", pic = "Calmogen" },
            };
        }

        return _medicamente;
    }
}
