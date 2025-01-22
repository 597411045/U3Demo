using System;
using System.Text;
using UnityEngine;

public class SaveManager : SingleTon<SaveManager>
{

    private static string saveSkillData = "";
    private static string savePropertyData = "";
    private static string saveplayerData = "";


    public void SaveData()
    {
        var list = EntityManager.Instance.playerList;

        StringBuilder skillData = new StringBuilder();
        foreach (var iter in list)
        {
            skillData.Append("<Character><Name>" + iter.gameObject.name + "<Name>");
            skillData.Append(iter.skillData.ExportData());
            skillData.Append("<Character>");
        }

        saveSkillData = skillData.ToString();

        StringBuilder propertyData = new StringBuilder();
        foreach (var iter in list)
        {
            propertyData.Append("<Character><Name>" + iter.gameObject.name + "<Name>");
            propertyData.Append(iter.characterData.ExportData());
            propertyData.Append("<Character>");
        }

        savePropertyData = propertyData.ToString();

        saveplayerData = PlayerController.Instance.ExportData();
    }

    public void ImportData()
    {
        var list = EntityManager.Instance.playerList;

        string data = saveSkillData;
        if (data != "")
        {
            var l1 = data.Split("<Character>");
            foreach (var iter1 in l1)
            {
                if (iter1.Contains("<Name>"))
                {
                    var l2 = iter1.Split("<Name>");
                    AIController tmpAic = null;
                    foreach (var iter2 in l2)
                    {
                        if (iter2.Contains("<SkillData>") && tmpAic != null)
                        {
                            tmpAic.skillData.ImportData(iter2);
                        }
                        else if (iter2 != "")
                        {
                            tmpAic = list.Find(x => x.gameObject.name == iter2);
                        }
                    }
                }
            }
        }

        data = savePropertyData;
        if (data != "")
        {
            var l1 = data.Split("<Character>");
            foreach (var iter1 in l1)
            {
                if (iter1.Contains("<Name>"))
                {
                    var l2 = iter1.Split("<Name>");
                    AIController tmpAic = null;
                    foreach (var iter2 in l2)
                    {
                        if (iter2.Contains("<PropertyData>") && tmpAic != null)
                        {
                            tmpAic.characterData.ImportData(iter2);
                        }
                        else if (iter2 != "")
                        {
                            tmpAic = list.Find(x => x.gameObject.name == iter2);
                        }
                    }
                }
            }
        }

        data = saveplayerData;
        if (data != "")
        {
            PlayerController.Instance.ImportData(data);
        }
    }
}