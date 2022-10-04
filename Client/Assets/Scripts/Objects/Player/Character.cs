using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Define;

public abstract class Character : MonoBehaviour, NetworkObject {
    #region Components & GameObjects
    [Header("Character Components")]

    #endregion

    #region Variables
    [SerializeField] protected PlayerStat       _stat;

    #endregion
    #region Properties
    public uint AuthCode { get; set; }
    public bool IsLocal { get; protected set; }
    #endregion

    #region Unity Event Functions

    /// <summary>
    /// Initializing First
    /// </summary>
    private void Awake() {
        OnAwakeEvent();
    }

    /// <summary>
    /// Initializing Second
    /// </summary>
    private void OnEnable() {
        OnEnableEvent();
    }

    /// <summary>
    /// Initializing Third
    /// </summary>
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
    protected virtual void OnEnableEvent() { }

    /// <summary>
    /// All Characters' Moving Method except for Local Player.
    /// </summary>
    /// <param name="serverTick">Dedicated Server's Tick for synchronization.</param>
    /// <param name="newPosition">Character's next position.</param>
    public virtual void Move(uint serverTick, Vector3 newPosition) { }
    public virtual void Rotate(Quaternion camFront) { }

    #endregion

    #region Abstract Functions
    /// <summary>
    /// All Character's DecreasingHealth Method.
    /// </summary>
    /// <param name="damage">value that calculated by Dedicated Server.</param>
    public abstract void OnDamage(float damage);
    public abstract void OnDeath();
    #endregion
}