using UnityEngine;

namespace MethodTypes
{
    public delegate void voidfunction();
    public delegate void voidfunctionTransform(Transform tr);
    public delegate void voidfunctionGameObject(GameObject gmbjct);
    public delegate bool boolfunction();
    public delegate bool boolfunctionbool(bool tf);
    public delegate bool boolfunctioninput(InputNM.Inputstruct input);
    public delegate bool boolfunctionKeycode(KeyCode input);
    public delegate Transform Transformfunction();
    public delegate GameObject GameObjectfunction();
    public delegate GameObject GameObjectfunctionTransform(Transform tr);
}

public static class MethodsNull
{

    public static void DO_NOTHING()
    {

    }
    public static bool DO_NOTHING_TRUE()
    {
        return true;
    }
    public static bool DO_NOTHING_FALSE()
    {
        return false;
    }

}