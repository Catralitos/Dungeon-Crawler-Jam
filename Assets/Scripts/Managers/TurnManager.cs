using System.Collections;
using System.Collections.Generic;
using Audio;
using System.Linq;
using Enemies;
using Items;
using Player;
using Puzzles;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// The TurnManager processes each turn of the game
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class TurnManager : MonoBehaviour
    {
        #region SingleTon

        /// <summary>
        /// Gets the sole instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static TurnManager Instance { get; private set; }

        /// <summary>
        /// Awakes this instance (if none have been created already).
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        #endregion

        /// <summary>
        /// The time it takes for a unit to move to a new position
        /// </summary>
        public float unitTimeToMove = 0.2f;
        
        /// <summary>
        /// The enemies in map
        /// </summary>
        
        /// <summary>
        /// The AudioManager
        /// </summary>
        [HideInInspector] public AudioManager audioManager;

        public List<StateMachine> enemiesInMap;
        
        /// <summary>
        /// The enemies in map
        /// </summary>
        public List<Portal> portalsInMap;
        
        /// <summary>
        /// The player start position
        /// </summary>
        private Vector3 _playerStartPosition;
        
        /// <summary>
        /// Gets a value indicating whether [processing turn].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [processing turn]; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessingTurn { get; private set; }

        /// <summary>
        /// Gets the current turn.
        /// </summary>
        /// <value>
        /// The current turn.
        /// </value>
        public int CurrentTurn { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            //Create the lists if they haven't been created yet 
            enemiesInMap ??= new List<StateMachine>();
            portalsInMap ??= new List<Portal>();
            audioManager = GetComponent<AudioManager>();
            //Start ambience sound in World 1
            audioManager.Play("world01-default-ambience");
        }

        /// <summary>
        /// Adds a zombie to the manager.
        /// </summary>
        /// <param name="g">The zombie.</param>
        public void AddZombie(StateMachine g)
        {
            enemiesInMap ??= new List<StateMachine>();
            enemiesInMap.Add(g);
        }
        
        /// <summary>
        /// Gets the current number of zombies.
        /// </summary>
        /// <returns>Number of zombies</returns>
        public int GetNumberOfZombies()
        {
            return enemiesInMap.Count;
        }

        /// <summary>
        /// Processes the turn.
        /// </summary>
        /// <param name="playerPos">The player's position.</param>
        public void ProcessTurn(Vector3 playerPos)
        {
            if (ProcessingTurn) return;
            ProcessingTurn = true;
            StartCoroutine(TurnCoroutine(playerPos));
        }

        /// <summary>
        /// A coroutine that performs everything required to conclude a turn
        /// </summary>
        /// <param name="playerPos">The player's position.</param>
        /// <returns></returns>
        private IEnumerator TurnCoroutine(Vector3 playerPos)
        {
            //Move all the zombies
            foreach (var z in enemiesInMap) z.RunStateMachine(playerPos);

            //Wait until they have finished moving
            yield return new WaitForSeconds(unitTimeToMove);
            
            Portal toDestroy = null;
            
            foreach (Portal p in portalsInMap.Where(p => p.hasPlayer))
            {
                if (PlayerEntity.Instance.transform.position.y > 5)
                {
                    PlayerEntity.Instance.transform.position += Vector3.down * 10; // go to world 1
                    //Check world 2 ambience sound
                    if(audioManager.IsPlaying("world02-default-ambience"))
                        audioManager.Stop("world02-default-ambience");
                    //Play world 1 ambience sound
                    audioManager.Play("world01-default-ambience");
                }
                else
                {
                    PlayerEntity.Instance.transform.position += Vector3.up * 10; // go to world 2
                    //Check world 1 ambience sound
                    if(audioManager.IsPlaying("world01-default-ambience"))
                        audioManager.Stop("world01-default-ambience");
                    //Play world 2 ambience sound
                    audioManager.Play("world02-default-ambience");
                }

                if (p.invisible)
                {
                    toDestroy = p;
                }
            }

            if (toDestroy != null)
            {
                portalsInMap.Remove(toDestroy);
                Destroy(toDestroy.gameObject);
            }

            //Increase the number of turns, and if the right amount has passed, take damage from lack of sleep
            CurrentTurn++;

            ProcessingTurn = false;
        }

        /// <summary>
        /// Determines whether this instance can move.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can move; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMove()
        {
            return !ProcessingTurn && !EntitiesAreMoving();
        }

        /// <summary>
        /// Check if the player or zombies are moving
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the player or a zombie is still moving; otherwise, <c>false</c>.
        /// </returns>
        private bool EntitiesAreMoving()
        {
            foreach (var z in enemiesInMap)
                if (z.currentState.isActing)
                    return true;

            return PlayerEntity.Instance.movement.IsMoving;
        }
    }
}