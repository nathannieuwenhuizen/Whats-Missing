using UnityEngine;

public class ChangeableObject : MonoBehaviour, IChangable
{
    [SerializeField]
    private string word;
    public string Word {
        get { return word;}
        set {word = value;}
    }

    public void setChange(ChangeType changeType) {
        switch (changeType) {
            case ChangeType.missing:
                onMissing();
                break;
        }
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