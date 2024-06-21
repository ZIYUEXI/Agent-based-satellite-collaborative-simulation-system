using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DropdownPopulator : MonoBehaviour
{
    public TMP_Dropdown dropdown; // 在Inspector中指定
    private List<string> currentOptions = new List<string>(); // 存储当前下拉菜单中的选项
    public float updateInterval = 1.0f; // 更新间隔，可以在Inspector中调整
    private float timer; // 计时器

    void Start()
    {
        PopulateDropdown();
    }

    void Update()
    {
        // 更新计时器
        timer += Time.deltaTime;

        // 当达到更新间隔时，重新填充下拉菜单
        if (timer >= updateInterval)
        {
            timer = 0;
            PopulateDropdown();
        }
    }

    void PopulateDropdown()
    {
        // 查找所有带有"SpaceObject"标签的游戏对象
        GameObject[] spaceObjects = GameObject.FindGameObjectsWithTag("SpaceObject");
        List<string> newOptions = new List<string>();

        // 遍历所有SpaceObject，将它们的名字加入到新的选项列表中
        foreach (var obj in spaceObjects)
        {
            newOptions.Add(obj.name);
        }

        // 如果新的选项列表和当前列表不同，则更新下拉菜单
        if (!AreListsEqual(newOptions, currentOptions))
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
            currentOptions = newOptions;
        }
    }

    // 检查两个列表是否相等
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
