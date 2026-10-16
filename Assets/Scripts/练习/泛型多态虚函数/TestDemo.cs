using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestDemo : MonoBehaviour
{
    
    void Start()
    {
        Debug.Log("=====测试虚函数+多态=====");
        Debug.Log("=====父类引用指向子类继承覆写（多态调用可通过父类获取 `Animal dog = new Dog()`）=====");
        Animal a1 = new Dog();//实例化一个狗对象，赋值给a1//**只能访问父类里有的成员**，子类新增的方法看不到
        Animal a2 = new Cat();//实例化一个猫对象，赋值给a2
        a1.Speak();//可以！父类虚方法，执行Dog的Speak（多态）//override重写的Speak方法，输出“汪汪汪”
        a2.Speak();
        Debug.Log("=====子类独有方法 不能通过 Animal 引用调用，只能通过 Dog 引用调用 =====");
        //变量类型写 Dog，才能调用 Dog 类的独有方法
        Dog dog = new Dog();//实例化一个狗对象，赋值给dog
        dog.name = "旺财";//name是父类Animal的属性，用于存储动物的名称,Dog类继承了Animal的属性所以可以使用
        dog.age = 1;
        dog.ShowName();//子类独有的函数 
        dog.ShowAge();//子类独有的函数


        Debug.Log("=====测试普通泛型盒子=====");
        //T = int
        Box<int> inBox = new Box<int>();//实例化一个整数类型的盒子，赋值给inBox
        inBox.item = 100;//通过inBox地址获取内存里实例化的Box地址并将100赋值给item
        inBox.Show();//调用执行Box里的Show函数，输出“盒子里的内容：100”
        //T = string
        Box<string> strBox = new Box<string>();//实例化一个字符串类型的盒子，赋值给strBox
        strBox.item  = "hello world";
        strBox.Show();//调用执行Box里的Show函数，输出“盒子里的内容：hello world”

        Debug.Log("=====测试动物泛型盒子+多态=====");
        //泛型容器通过 where T : Animal 限制只能存储动物类型的子对象
        AnimalBox<Dog> dogBox = new AnimalBox<Dog>();//实例化一个狗类型的盒子，赋值给dogBox
        dogBox.animal = new Dog();//实例化一个狗对象，赋值给dogBox中的动物
        dogBox.LetTalk();//调用狗的Speak方法，输出“汪汪汪”

        AnimalBox<Cat> catBox = new AnimalBox<Cat>();//实例化一个猫类型的盒子，赋值给catBox
        catBox.animal = new Cat();//实例化一个猫对象，赋值给catBox中的动物
        catBox.LetTalk();//调用猫的Speak方法，输出“喵喵喵”



    }
    

    
}
