using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class WriteData : MonoBehaviour
{

    void Start()
    {
        //Init();
    }

    public void Init()
    {
        var simtome = Data.Instance.GetSimtoms();
        var medicamente = Data.Instance.GetMedicamente();

        var trainingCsv = new StringBuilder();
        var testCsv = new StringBuilder();

        trainingCsv.AppendLine("simtom_id, simtom, pastila_id, pastila, rating");
        testCsv.AppendLine("simtom_id, simtom, pastila_id, pastila, rating");

        foreach (var simtom in simtome)
        {
            var i = 0;
            foreach (var medic in medicamente)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4}", simtom.id, simtom.nume, medic.id, medic.nume, (int)Random.Range(1f, 6f));
                if (i == 0)
                {
                    testCsv.AppendLine(newLine);
                    i++;
                }
                trainingCsv.AppendLine(newLine);
            }
        }

        var trainingFilePath = string.Format(@"Assets/StreamingAssets/{0}", Data.Instance.TraingDataPath);
        var testingFilePath = string.Format(@"Assets/StreamingAssets/{0}", Data.Instance.TestDataPath);

        File.WriteAllText(trainingFilePath, trainingCsv.ToString());
        File.WriteAllText(testingFilePath, testCsv.ToString());
    }

    public void MedicamentationDangers()
    {
        (List<string> trainingRows, List<string> testingRows) = GetDatas();

        var trainingCsv = new StringBuilder();
        var testCsv = new StringBuilder();

        trainingCsv.AppendLine("Pastile, Periculos");
        testCsv.AppendLine("Pastile, Periculos");

        foreach (var row in trainingRows)
        {
            var newLine = string.Format("\"{0}\"", row);
            newLine += ", " + (((int)Random.Range(0, 2)) >= 1).ToString();
            trainingCsv.AppendLine(newLine);
        }

        foreach (var row in testingRows)
        {
            var newLine = string.Format("\"{0}\"", row);
            newLine += ", " + (((int)Random.Range(0, 2)) >= 1).ToString();
            testCsv.AppendLine(newLine);
        }

        var trainingFilePath = string.Format(@"Assets/StreamingAssets/{0}", Data.Instance.TraingDangerousDataPath);
        var testingFilePath = string.Format(@"Assets/StreamingAssets/{0}", Data.Instance.TestDangerousDataPath);

        File.WriteAllText(trainingFilePath, trainingCsv.ToString());
        File.WriteAllText(testingFilePath, testCsv.ToString());
    }

    private (List<string>, List<string>) GetDatas()
    {
        var medicamente = Data.Instance.GetMedicamente();

        List<string> rows = new List<string>();

        foreach (var med1 in medicamente)
        {
            foreach (var med2 in medicamente)
            {
                if (med1.id == med2.id)
                    continue;

                var exists = CheckBeforeAdd(med1, med2, rows);

                if (exists)
                    continue;

                rows.Add(med1.nume + ", " + med2.nume);
            }
        }

        medicamente = medicamente.OrderByDescending(m => m.id).ToList();

        for (var i = 0; i < rows.Count; i++)
        {
            if (rows[i].Contains(medicamente[i].nume) == false)
            {
                rows[i] += ", " + medicamente[i].nume;
            }

            if (i == 15)
                break;
        }

        //foreach (var med1 in medicamente)
        //{
        //    foreach (var med2 in medicamente)
        //    {
        //        if (med1.id == med2.id)
        //            continue;

        //        foreach (var med3 in medicamente)
        //        {
        //            var existsIndex = CheckBeforeAdd(med1, med2, med3, rows);

        //            if (existsIndex >= 0)
        //            {
        //                rows.RemoveAt(existsIndex);
        //            }
        //        }
        //    }
        //}

        foreach (var row in rows)
        {
            Debug.Log(row);
        }

        List<string> testRows = new List<string>();
        List<int> randomSelected = new List<int>();

        while (randomSelected.Count < 20)
        {
            var randomNumber = (int)Random.Range(0f, rows.Count);

            if (randomSelected.FindIndex(r => r == randomNumber) >= 0)
                continue;

            testRows.Add(rows[randomNumber]);
            randomSelected.Add(randomNumber);
        }

        return (rows, testRows);
    }

    private bool CheckBeforeAdd(Medicament med1, Medicament med2, List<string> rows)
    {
        if (med1.id == med2.id)
            return true;

        foreach (var row in rows)
        {
            if (
                (row.Contains(med1.nume) && row.Contains(med2.nume))
                )
            {
                return true;
            }
        }

        return false;
    }

    private int CheckBeforeAdd(Medicament med1, Medicament med2, Medicament med3, List<string> rows)
    {
        if (
            med2.id == med3.id
            ||
            med1 == med3
            ||
            med1 == med2
            )
            return -1;

        var i = 0;
        foreach (var row in rows)
        {
            if (
                (row.Contains(med1.nume) && row.Contains(med2.nume) && row.Contains(med3.nume))
                )
            {
                return i;
            }
            i++;
        }
        return -1;
    }

}

public class Simtom
{
    public int id;
    public string nume;
}

public class Medicament
{
    public int id;
    public string nume;
    public string pic;

    public double Score;
}

public interface IMedicamentComponent
{
    int Id { get; set; }
    GameObject GameObject { get; }
}
