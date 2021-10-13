

using System.Collections;

public interface Interactable
{
    void Interact();

    string TextOnScreen { get; }
}


public static class MyInterfaceExtensions
{


    public static bool IEnumerableHasCountOver(this IEnumerable ienumerable , int n)
    {
        int i = 0;
        foreach(var _ in ienumerable)
        {
            if(++i > n)
            {
                return true;
            }
        }
        return false;
    }

}