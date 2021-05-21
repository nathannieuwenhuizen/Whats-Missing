using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    public static string[] firstLines = {"Psst!", "Hey you!", "I see you're bored out of your mind being in this waiting room.", "Would you like to play a little game?"};
    public static string[] responseToNoGame = {"Oh... uhm...", "Doesn't matter, we're still going to play."};
    public static string[] responseToGame = {"Awesome!"};
    public static string[] askForName = {"May I ask what your name is?"};
    public static string[] responceofSmallName = {"Aha... great you've used a joke name, I can live with that."};
    public static string[] responceofTooLongName = {"There's no way I can remember that, I'll just cal you [NAME]."};
    public static string[] responceofNormalName = {"Nice to meet you [NAME]!"};
    public static string[] goToNextRoom = {"Let's begin, go through the door on your right ->"};

    public static string[] firstRoom = {"Look at the painting.", "Memorize it well!", "Alright, go to the next room."};
    public static string[] secondRoom = {"Now tell me..." , "What's missing?"};
}
