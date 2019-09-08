using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosticController : MonoBehaviour
{
    //public Text Text;
    public SimtomeController SimtomeController;
    public Text ActionButtonText;

    private List<Medicament> _medicamentePropuse;
    private List<IPrefabComponent> _medicamentsPool;
    public RectTransform MedicamentParent;

    public GameObject MedicamentPrefab;

    private bool _actionIsCalculate = false;

    public void OnAction()
    {
        _actionIsCalculate = !_actionIsCalculate;

        if (_actionIsCalculate)
        {
            CalculateDiagnostic();
        }
        else
        {
            Data.Instance.UserSimtoms = new List<Simtom>();
            SimtomeController.RefreshOptions();
            SimtomeController.ShowSimtoms();
        }
    }

    public void CalculateDiagnostic()
    {
        string message = string.Empty;
        bool recomended = false;
        double score;

        _medicamentePropuse = new List<Medicament>();

        foreach (Simtom simtom in Data.Instance.UserSimtoms)
        {
            foreach (Medicament medicament in Data.Instance.GetMedicamente())
            {
                (message, recomended, score) = MLFarmacie.Instance.MakePrediction(simtom, medicament);

                if (recomended == true)
                {
                    if (_medicamentePropuse.FindIndex(m => m.id == medicament.id) < 0)
                    {
                        medicament.Score = score;
                        _medicamentePropuse.Add(medicament);
                    }
                }
            }
        }

        _medicamentePropuse = _medicamentePropuse.OrderByDescending(m => m.Score).ToList();

        //Text.text = Environment.NewLine + " Primul rand de calcule" + Environment.NewLine;
        //foreach (Medicament med in _medicamentePropuse)
        //{
        //    Text.text += Environment.NewLine + med.nume + "       Score: " + med.Score;
        //}

        StartCoroutine(SecondPhase());
    }

    IEnumerator SecondPhase()
    {
        yield return new WaitForSeconds(1f);

        if (MLFarmacie.Instance.DangerousEngineCreated == false)
            MLFarmacie.Instance.CreateDangerousEngine();

        double additionalScores = 0;
        foreach (Medicament med in _medicamentePropuse)
        {
            additionalScores += med.Score;
        }
        additionalScores = additionalScores / _medicamentePropuse.Count;

        for (var i = 0; i < _medicamentePropuse.Count; i++)
        {
            var dangerous = MLFarmacie.Instance.MakeDangerousPrediction(_medicamentePropuse[i].nume);

            if (dangerous == false)
            {
                _medicamentePropuse[i].Score += additionalScores;
            }
        }

        //-----

        Dictionary<string, double> combinations = new Dictionary<string, double>();

        foreach (Medicament med1 in _medicamentePropuse)
        {

            foreach (Medicament med2 in _medicamentePropuse)
            {
                if (med1.id == med2.id)
                    continue;

                if (combinations.ContainsKey(med1.nume + ", " + med2.nume) == false)
                {
                    combinations.Add(med1.nume + ", " + med2.nume, med1.Score + med2.Score);
                }
            }
        }

        //combinations = combinations.OrderBy(m => m.Value);
        string allNonDangerousCombinations = string.Empty;

        foreach (KeyValuePair<string, double> comb in combinations)
        {
            var dangerous = MLFarmacie.Instance.MakeDangerousPrediction(comb.Key);

            if (dangerous == false)
            {
                allNonDangerousCombinations += comb.Key;
            }
        }

        foreach (Medicament med in _medicamentePropuse)
        {
            var addScore = CountStringOccurrences(allNonDangerousCombinations, med.nume);
            var newScore = Math.Round(med.Score + (additionalScores * (addScore / combinations.Count)), 2, MidpointRounding.AwayFromZero);
            med.Score = newScore;
        }

        _medicamentePropuse = _medicamentePropuse.OrderByDescending(m => m.Score).ToList();

        OnMedicamentePropuse();

        //Text.text += Environment.NewLine + Environment.NewLine + " Al doilea rand de calcule" + Environment.NewLine;
        //foreach (Medicament med in _medicamentePropuse)
        //{
        //    Text.text += Environment.NewLine + med.nume + "       Score: " + med.Score;
        //}

        ActionButtonText.text = "De la inceput";
    }

    public void OnMedicamentePropuse()
    {
        foreach (Medicament medicament in _medicamentePropuse)
        {
            var wasNull = UsefullUtils.CheckInPool(
                medicament.id,
                MedicamentPrefab,
                MedicamentParent.transform,
                out IPrefabComponent medicamentComponent,
                ref _medicamentsPool
                );

            medicamentComponent.Id = medicament.id;
            
            medicamentComponent.GameObject.name = medicament.nume;
            (medicamentComponent as MedicamentComponent).Name.text = medicament.nume;

            Debug.Log(medicament.nume);

            Sprite sprite = Resources.Load("Images/" + medicament.pic, typeof(Sprite)) as Sprite;
            (medicamentComponent as MedicamentComponent).SetImage(sprite);

            if (wasNull)
            {
                _medicamentsPool.Add(medicamentComponent);
            }
        }

        var newHeight = (_medicamentePropuse.Count / 2) * 440;
        MedicamentParent.sizeDelta = new Vector2(MedicamentParent.sizeDelta.x, newHeight);
    }

    public int CountStringOccurrences(string text, string pattern)
    {
        // Loop through all instances of the string 'text'.
        int count = 0;
        int i = 0;
        while ((i = text.IndexOf(pattern, i)) != -1)
        {
            i += pattern.Length;
            count++;
        }
        return count;
    }

}
