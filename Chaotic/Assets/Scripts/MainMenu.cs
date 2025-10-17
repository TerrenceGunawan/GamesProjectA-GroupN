using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject controllerScreen;
    [SerializeField] GameObject loadGameButton;
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        loadGameButton.SetActive(File.Exists(SavePath));
    }

    public void StartGame()
    {
        controllerScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public static void LoadGame()
    {
        if (!File.Exists(SavePath)) { Debug.LogWarning("No save file"); return; }

        string json = File.ReadAllText(SavePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        // Load the correct scene asynchronously
        SceneManager.LoadSceneAsync(data.sceneName).completed += op =>
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            Player player = FindFirstObjectByType<Player>();
            player.Sanity = data.sanity;
            player.Inventory = new List<string>(data.inventory);
            player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            player.transform.rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);

            ItemInteract[] allItems = FindObjectsByType<ItemInteract>(FindObjectsSortMode.None);
            foreach (ItemTransformData itemData in data.itemTransforms)
            {
                foreach (ItemInteract item in allItems)
                {
                    if (item.name == itemData.itemName)
                    {
                        item.transform.position = new Vector3(itemData.position[0], itemData.position[1], itemData.position[2]);
                        break;
                    }
                }
            }

            foreach (string itemName in player.Inventory)
            {
                foreach (GameObject item in gm.PlayerInventory)
                {
                    if (item.name == itemName)
                    {
                        Destroy(item);
                        break;
                    }
                }
            }
            foreach (var checker in gm.checkers)
                if (data.completedPuzzles.Contains(checker.name) && !data.completedObjects.Contains(checker.name))
                    checker.HasSucceeded = true;

            foreach (var icc in gm.itemCheckerCheckers)
                if (data.completedPuzzles.Contains(icc.name))
                    icc.AllItemsChecked = true;

            foreach (var kp in gm.keypads)
                if (data.completedPuzzles.Contains(kp.name))
                    kp.Completed = true;

            foreach (var pc in gm.patternCheckers)
                if (data.completedPuzzles.Contains(pc.name))
                    pc.Completed = true;

            foreach (var js in gm.jumpscares)
                if (data.completedObjects.Contains(js.name))
                    js.Played = true;

            foreach (var phone in gm.phones)
            {
                if (data.completedObjects.Contains(phone.name))
                    phone.PickedUp = true;
                if (data.completedPhoneTwice.Contains(phone.name))
                    phone.PickedUpTwice = true;
            }
        };
    }

    public void ClickStart()
    {
        SceneManager.LoadScene("Basement");
    }
     
}
