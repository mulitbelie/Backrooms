using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 实现功能：数组和列表
/// </summary>
public class ArrayAndList : MonoBehaviour
{
    //需要指定数组的类型和元素数量
    // 数组特点 ：
    // 1. 长度固定（无法修改）
    // 2. 访问元素速度比列表快
    // 3. 功能有限（只能储存访问元素，不能添加删除元素）
    // 4. 内存占用结构简单（适合处理已知大小的数据集）
    // 5. 内存使用效率高（固定大小）
    int[] array = new int[5];//创建一个5个元素的整数数组
    // int[] array = {1, 2, 3, 4, 5};//初始化数组
    
    //列表的类型和元素数量可以不指定，但是需要在使用时指定
    // 列表特点 ：
    // 1. 长度可变（可以添加删除元素）
    // 2. 访问元素速度比数组慢（动态调整）
    // 3. 功能丰富（可以添加删除元素，可以遍历查找元素）
    // 4. 内存占用结构复杂（需要动态分配内存）
    // 5. 内存使用效率低（动态分配内存，需要考虑内存碎片）
    List<int> list = new List<int>();//创建一个空的整数列表
    // List<int> list = new List<int>{1, 2, 3, 4, 5};//初始化列表

    void Start()
    {
        //常用方法
        //数组
        //通过索引来访问数组元素，索引值从0开始0 1 2 3 4
        Debug.Log(array[0]);//输出数组的第一个元素
        //获取数组长度
        Debug.Log(array.Length);//输出数组的长度
        //数组
        TraverseArray();//遍历输出数组所有元素
        //--------------------------------------------------
        //列表
        //通过索引来访问列表元素，索引值从0开始0 1 2 3 4
        //【访问元素】根据索引值，输出列表对应元素
        Debug.Log(list[0]);
        //【添加元素】添加一个元素6到列表的末尾
        list.Add(6);
        //【插入元素】在索引值为1的位置插入一个元素7
        list.Insert(1, 7);
        //【删除元素】删除列表中指定的元素
        list.Remove(3);
        //【删除元素】删除列表中索引值指定的元素
        list.RemoveAt(3);
        //【获取元素数量】获取列表中元素的数量
        Debug.Log(list.Count);
        //【清空元素】清空列表中的所有元素
        list.Clear();
        //【判断元素】判断列表中是否包含指定的元素
        Debug.Log(list.Contains(3));//输出false不包含元素，true包含元素
        //【遍历元素】遍历列表中的所有元素，输出列表所有元素
        TraverseList();
    }
    //遍历数组
    private void TraverseArray()
    {
        for(int i = 0; i<array.Length; i++)
        {
            Debug.Log(array[i]);//索引值为i的 输出数组的第i个元素
        }
    }
    //遍历列表
    private void TraverseList()
    {
        foreach(var item in list)//在列表中遍历每个元素item
        {
            Debug.Log(item);//输出列表的元素item
        }
    }
    
}