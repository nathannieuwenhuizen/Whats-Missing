using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : PickableRoomObjectThatplaysSound
{
    public static int ammountOfBooksBurned = 0;
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
    private void OnDestroy() {
        if (isBurning) {

            ammountOfBooksBurned++;

            // int ammountOfBooks = 0;
            // Debug.Log("check other books");
            // foreach(Book book in Room.FindObjectsOfType<Book>()) {
            //     if (book != this) ammountOfBooks++;
            // }
            Debug.Log("ammount of books remaining: " + ammountOfBooksBurned);
            if (ammountOfBooksBurned >= 20) {
                SteamAchievementHandler.Instance?.SetAchievement(SteamAchievement.TheLibraryIsClosed);
                foreach(Book book in Room.FindObjectsOfType<Book>()) {
                    if (book != this) {
                        Destroy(book.gameObject); 
                    }
                }
            }
        }
    }
}
