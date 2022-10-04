using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Character : MonoBehaviour, NetworkObject {
    #region Components & GameObjects
    [SerializeField] private PlayerStat       _stat;

    #endregion

    #region Properties
    public uint AuthCode { get; set; }
    public bool IsLocal { get; protected set; }

    protected PlayerStat Stat { get => _stat; }
    
    #endregion

    #region Unity Event Functions

    private void Awake() {
        OnAwakeEvent();
    }
    private void Start() {
        OnStartEvent();
    }

    #endregion

    #region Virtual Functions
    /// <summary>
    /// Calling by UnityEvent.Awake().
    /// </summary>
    protected virtual void OnAwakeEvent() { }

    /// <summary>
    /// Calling by UnityEvent.Start().
    /// </summary>
    protected virtual void OnStartEvent() { }

    /// <summary>
    /// All Characters' Moving Method except for Local Player.
    /// </summary>
    /// <param name="serverTick">Dedicated Server's Tick for synchronization.</param>
    /// <param name="newPosition">Character's next position.</param>
    public virtual void Move() { }

    #endregion

    #region Abstract Functions
    /// <summary>
    /// All Character's DecreasingHealth Method.
    /// </summary>
    /// <param name="damage">value that calculated by Dedicated Server.</param>
    public abstract void SetCharacterHealth(float damage);
    public abstract void OnDeath();
    #endregion
}