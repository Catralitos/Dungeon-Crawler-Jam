using System.Collections.Generic;
using Audio;
using Enemies;
using Items;
using Maps;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Grid = Maps.Grid<Maps.GridCell<bool>>;

namespace Managers
{
    /// <summary>
    /// The GameManager handles loading scenes, generating new levels and other high-level tasks
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class GameManager : MonoBehaviour
    {
        #region SingleTon

        /// <summary>
        /// Gets the sole instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static GameManager Instance { get; private set; }
        [HideInInspector] public string savePath;

        /// <summary>
        /// Awakes this instance (if none already exists).
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                savePath = Application.persistentDataPath + "/player.resi";
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        #endregion
        
        /// <summary>
        /// The AudioManager
        /// </summary>
        [HideInInspector] public AudioManager audioManager;

        /// <summary>
        /// The player prefab
        /// </summary>
        [Header("Prefabs")] public GameObject playerPrefab;
        

        /// <summary>
        /// The hud UI
        /// </summary>
        [Header("UI Elements")] public GameObject hudUI;
        /// <summary>
        /// The shop UI
        /// </summary>
        public GameObject itemBoxUI;

        /// <summary>
        /// The player start position
        /// </summary>
        private Vector3 _playerStartPosition;
        

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            //audioManager = GetComponent<AudioManager>();
            //audioManager.Play("Music");
        }
        
        public void LoadTitleScreen()
        {
            SceneManager.LoadScene(0);
        }
    
        public void LoadMainScene()
        {
            SceneManager.LoadScene(1);
        }
        
        public void LoadCredits()
        {
            SceneManager.LoadScene(2);
        }
    
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}