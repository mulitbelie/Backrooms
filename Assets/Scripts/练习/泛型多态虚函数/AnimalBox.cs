using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBox<T> where T : Animal
{
    public T animal;
    public void LetTalk()
    {
        animal.Speak();
    }
}
