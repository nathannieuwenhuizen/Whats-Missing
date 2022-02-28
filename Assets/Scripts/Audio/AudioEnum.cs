using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class SFXFiles {

    //door
    public static string door_closing = "event:/SFX/Door/Door_shut";
    public static string door_open = "event:/SFX/Door/Door_open";
    public static string door_squeek = "event:/SFX/Door/Door_squeek";
    public static string door_locked = "event:/SFX/Door/Door_locked";

    //player
    public static string player_jump = "event:/SFX/Player/Jump";
    public static string player_landing = "event:/SFX/Player/Landing";
    public static string player_footstep_normal = "event:/SFX/Player/Footstep";
    public static string player_choking = "event:/SFX/Player/Choking";
    public static string player_relief_gasp = "event:/SFX/Player/Relief_gasp";
    public static string choke_die = "event:/SFX/Player/Choke_die";
    public static string heartbeat = "event:/SFX/Player/Heartbeat";
    public static string wind_fall = "event:/SFX/Player/Wind_fall";
    public static string exhale = "event:/SFX/Player/Exhale";
    public static string player_hits_ground = "event:/SFX/Player/Falling";
    public static string player_cough = "event:/SFX/Player/Coughing";
    public static string player_grab = "event:/SFX/Player/Grab";
    public static string player_footstep_water = "Player_footstep_water";
    public static string player_footstep_grass = "Player_footstep_grass";
    public static string stairs_footstep = "Stairs_footstep";

    
    //planetarium
    public static string letter_click = "event:/SFX/Mirror/Letter_click";
    public static string mirror_true = "event:/SFX/Mirror/Mirror_turning_on";
    public static string mirror_false = "event:/SFX/Mirror/Mirror_turning_off";
    public static string fire_crackling = "event:/SFX/Environment/Planetarium/Fire_crackling";
    public static string fire_spread_burning = "event:/SFX/Environment/Planetarium/Fire_spread_burning";

    public static string chair_hit = "Chair_hit";
    public static string clock_ticking = "event:/SFX/Environment/Planetarium/Clock_tick";
    public static string evil_spirit = "Evil_spirit";
    public static string lamp_toggle = "event:/SFX/Environment/Planetarium/Lamp_toggle";

    //hiddenroom
    public static string rumble_ground = "Rumble_ground";
    public static string woosh = "Woosh";
    public static string grab_book = "Grab_book";


    //ui
    public static string ui_button_click = "event:/SFX/UI/Button_click";
    public static string ui_button_hover = "event:/SFX/UI/Button_hover";
    public static string ui_button_unhover = "event:/SFX/UI/Button_unhover";
    public static string pause_show = "event:/SFX/UI/Pause_panel_show";
    public static string pause_hide = "event:/SFX/UI/Pause_panel_close";
    public static string play_button_click = "event:/SFX/UI/Play_button_click";
    public static string toggle_on = "event:/SFX/UI/Toggle_on";
    public static string toggle_off = "event:/SFX/UI/Toggle_off";
    public static string slider_select = "event:/SFX/UI/Slider_select";
    public static string windows_error = "event:/SFX/UI/Windows_error";
    public static string hintbutton_show = "";

    //Garden
    public static string sun_burning = "event:/SFX/Environment/Garden/Sun_burning";
    public static string duck = "event:/SFX/Environment/Garden/Duck";
    public static string baby_duck = "event:/SFX/Environment/Garden/Baby_ducks";
    public static string fountain = "event:/SFX/Environment/Garden/Fountain";
    public static string wind = "event:/SFX/Environment/Garden/Wind";


    //grave
    public static string knee_on_dirt = "Knee_on_dirt";
    public static string music_box = "Music_box";
    public static string music_box_on_ground = "Music_box_on_ground";
    public static string pulling_pocket = "Pulling_pocket";
    public static string gregory_cry = "Gregory_cry";


    //music
    public static string MENU = "event:/Music/Menu";
    public static string HIDDEN_ROOM = "event:/Music/Hidden_room";
    public static string Environment1 = "event:/Music/Environment1";
    public static string Environment2 = "event:/Music/Environment2";

}
public enum MusicFiles {
    planetarium,
    menu,
    planetarium_hidden_room,
    garden
}
