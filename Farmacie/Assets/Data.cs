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
                new Medicament(){ id = 1, nume = "Brufen 400" },
                new Medicament(){ id = 2, nume = "Nurofen Express Forte" },
                new Medicament(){ id = 3, nume = "Fasconal" },
                new Medicament(){ id = 4, nume = "Solpadeine" },
                new Medicament(){ id = 5, nume = "Ibusinus" },
                new Medicament(){ id = 6, nume = "Coldrex MaxGrip " },
                new Medicament(){ id = 7, nume = "Strepsils" },
                new Medicament(){ id = 8, nume = "ACC" },
                new Medicament(){ id = 9, nume = "Tusocalm" },
                new Medicament(){ id = 10, nume = "Bixtonim" },
                new Medicament(){ id = 11, nume = "Vibrocil" },
                new Medicament(){ id = 12, nume = "Imodium" },
                new Medicament(){ id = 13, nume = "Theraflu" },
                new Medicament(){ id = 14, nume = "Smecta" },
                new Medicament(){ id = 15, nume = "Meltus sirop" },
                new Medicament(){ id = 16, nume = "Aspirină" },
                new Medicament(){ id = 17, nume = "Hexoral Spray" },
                new Medicament(){ id = 18, nume = "Sennalax" },
                new Medicament(){ id = 19, nume = "Melatonină" },
                new Medicament(){ id = 20, nume = "Calmogen Plant" }
            };
        }

        return _medicamente;
    }
}
