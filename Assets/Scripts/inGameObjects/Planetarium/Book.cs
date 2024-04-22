using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : PickableRoomObjectThatplaysSound
{
    public static int ammountOfBooksBurned = 0;
    public static bool checAmmountOfBooksBurned = true;
    public override string AudioFile()
    {
        return SFXFiles.book;
    }
    public override float AudioVolume()
    {
        return .8f;
    }
    protected override void Awake() {
        ammountOfBooksBurned = 0;
        base.Awake();
        flamable = true;
        missingChangeEffect = MissingChangeEffect.dissolve;
    }

    private void Reset() {
        flamable = true;
        missingChangeEffect = MissingChangeEffect.dissolve;

        Word = "Book";
        AlternativeWords = new string[] { "books", "paper" };
    }
    public override IEnumerator Burn()
    {
        Room.allObjects.Remove(this);
        yield return base.Burn();
    }
    private void OnDestroy() {
        if (isBurning && checAmmountOfBooksBurned) {
            ammountOfBooksBurned++;
            Debug.Log("ammount of books remaining: " + ammountOfBooksBurned);
            if (ammountOfBooksBurned >= 15) { 
                checAmmountOfBooksBurned = false;
                SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.TheLibraryIsClosed);
                foreach(Book book in Room.FindObjectsOfType<Book>()) {
                    if (book != this && book.inSpace) {
                        Room.allObjects.Remove(book);
                        Destroy(book.gameObject);
                    }
                }
            }
        }
    }
}
