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
}
