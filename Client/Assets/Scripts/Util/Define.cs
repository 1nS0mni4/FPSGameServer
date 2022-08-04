using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum CharacterType {
        MyPlayer,
        Player,

    }
    public enum UIManagerType {
        None,
        Main,
        Login,
        Lobby,
        Loading,
        Hideout,
        Fieldmap,
    }

    public enum PlayerInScene {
        None,
        Hideout,
        Fieldmap
    }

    public enum InteractType {
        None,

        Chest_Open,
        Chest_Close,

        Door_Open,
        Door_Close,
        Door_Breach,

        Item_Take,
        Item_Search,

        Object_Use,
    }
}
