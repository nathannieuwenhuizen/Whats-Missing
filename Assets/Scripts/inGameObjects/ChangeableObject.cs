using UnityEngine;

public class ChangeableObject : MonoBehaviour, IChangable
{
    [SerializeField]
    private string word;
    public string Word {
        get { return word;}
        set {word = value;}
    }

    public void onAppearing()
    {
        throw new System.NotImplementedException();
    }

    public void onMissing()
    {
        throw new System.NotImplementedException();
    }
}