using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DropdownPopulator : MonoBehaviour
{
    public TMP_Dropdown dropdown; // ��Inspector��ָ��
    private List<string> currentOptions = new List<string>(); // �洢��ǰ�����˵��е�ѡ��
    public float updateInterval = 1.0f; // ���¼����������Inspector�е���
    private float timer; // ��ʱ��

    void Start()
    {
        PopulateDropdown();
    }

    void Update()
    {
        // ���¼�ʱ��
        timer += Time.deltaTime;

        // ���ﵽ���¼��ʱ��������������˵�
        if (timer >= updateInterval)
        {
            timer = 0;
            PopulateDropdown();
        }
    }

    void PopulateDropdown()
    {
        // �������д���"SpaceObject"��ǩ����Ϸ����
        GameObject[] spaceObjects = GameObject.FindGameObjectsWithTag("SpaceObject");
        List<string> newOptions = new List<string>();

        // ��������SpaceObject�������ǵ����ּ��뵽�µ�ѡ���б���
        foreach (var obj in spaceObjects)
        {
            newOptions.Add(obj.name);
        }

        // ����µ�ѡ���б�͵�ǰ�б�ͬ������������˵�
        if (!AreListsEqual(newOptions, currentOptions))
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
            currentOptions = newOptions;
        }
    }

    // ��������б��Ƿ����
    bool AreListsEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count)
            return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
                return false;
        }
        return true;
    }
}
