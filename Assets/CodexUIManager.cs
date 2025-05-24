using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CodexUIManager : MonoBehaviour
{
    [Header("Managers")]
    public CodexManager codexManager;

    [Header("Panels")]
    public GameObject codexPanel;
    public GameObject menuPage;
    public GameObject entryPage;

    [Header("UI Containers")]
    public GameObject categoryButtonsContainer;
    public GameObject entryBackButton;

    [Header("UI Elements")]
    public TMP_Text selectedCategoryTitle;
    public TMP_Text entryTitle;
    public TMP_Text entryDescription;
    public Image entryImage;
    public Sprite defaultImage;

    [Header("Prefabs")]
    public GameObject categoryButtonPrefab;
    public GameObject topicPanelPrefab;
    public GameObject entryButtonPrefab;

    [Header("Layout Containers")]
    public Transform categoryContainer;
    public Transform topicContainer;

    private string currentCategoryName = "Codex";
    private List<Toggle> activeTopicToggles = new List<Toggle>();

    void Start()
    {
        codexPanel.SetActive(false);
        entryPage.SetActive(false);
        menuPage.SetActive(true);
        entryBackButton.SetActive(false);
        selectedCategoryTitle.text = currentCategoryName;

        GenerateCategories();
        Debug.Log("Categories loaded: " + codexManager.codexData.categories.Count);
    }

    public void ToggleCodex()
    {
        codexPanel.SetActive(!codexPanel.activeSelf);
    }

    void GenerateCategories()
    {
        bool first = true;

        foreach (var category in codexManager.codexData.categories)
        {
            var localCategory = category;

            GameObject button = Instantiate(categoryButtonPrefab, categoryContainer);
            button.GetComponentInChildren<TMP_Text>().text = localCategory.name;

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Category button clicked: " + localCategory.name);
                currentCategoryName = localCategory.name;
                selectedCategoryTitle.text = localCategory.name;

               
                entryPage.SetActive(false);
                menuPage.SetActive(true);
                entryBackButton.SetActive(false);
                Debug.Log("Showing categoryButtonsContainer — from gemerate categories");
                categoryButtonsContainer.SetActive(true);

                GenerateTopics(localCategory);
            });



            if (first)
            {
                currentCategoryName = localCategory.name;
                selectedCategoryTitle.text = localCategory.name;
                GenerateTopics(localCategory);
                first = false;
            }
        }
    }


    void GenerateTopics(Category category)
    {
        Debug.Log("GenerateTopics() was called");

        ClearContainer(topicContainer);
        activeTopicToggles.Clear();

        foreach (var topic in category.topics)
        {
            GameObject topicPanel = Instantiate(topicPanelPrefab, topicContainer);

            // Find required components
            Toggle topicToggle = topicPanel.transform.Find("TopicToggle")?.GetComponent<Toggle>();
            if (topicToggle == null)
            {
                Debug.LogError("TopicToggle not found or not a Toggle component!");
            }

            TMP_Text topicLabel = topicToggle.GetComponentInChildren<TMP_Text>();
            Transform entryButtonsContainer = topicPanel.transform.Find("EntryButtonsContainer");

            // Set default states
            topicToggle.isOn = false;
            entryButtonsContainer.gameObject.SetActive(false);
            topicLabel.text = topic.name;

            // Clear previous listeners for safety
            topicToggle.onValueChanged.RemoveAllListeners();

            topicToggle.onValueChanged.AddListener((bool isOn) =>
            {
                Debug.Log($"Toggle {topic.name} changed to {isOn}");
                entryButtonsContainer.gameObject.SetActive(isOn);

                if (isOn)
                {
                    foreach (var toggle in activeTopicToggles)
                    {
                        if (toggle != topicToggle)
                        {
                            toggle.isOn = false;
                        }
                    }
                }
            });


            activeTopicToggles.Add(topicToggle);

            // Create entry buttons
            foreach (var entry in topic.entries)
            {
                GameObject entryButton = Instantiate(entryButtonPrefab, entryButtonsContainer);
                entryButton.GetComponentInChildren<TMP_Text>().text = entry.title;

                entryButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ShowEntry(entry);
                });
            }
        }
    }

    void ShowEntry(Entry entry)
    {
        Debug.Log("ShowEntry() called for entry: " + entry.title);

        // Set text fields
        entryTitle.text = entry.title;
        entryDescription.text = entry.description;
        selectedCategoryTitle.text = entry.title;

        // Try to load the image
        Debug.Log("Trying to load image: " + entry.image);
        Sprite loaded = null;

        if (!string.IsNullOrEmpty(entry.image))
        {
            loaded = Resources.Load<Sprite>("Images/" + entry.image);
            Debug.Log(loaded != null ? " Loaded image: " + entry.image : " Failed to load: " + entry.image);
        }

        // Assign sprite (use default if load failed)
        entryImage.sprite = loaded != null ? loaded : defaultImage;
        Debug.Log(entryImage.sprite != null ? " Final image assigned" : " entryImage.sprite is null!");

        // Hide all category buttons inside the container
        foreach (Transform child in categoryButtonsContainer.transform)
        {
            child.gameObject.SetActive(false);
        }

        // Show the back button
        entryBackButton.SetActive(true); 

        // Switch UI panels
        menuPage.SetActive(false);
        entryPage.SetActive(true);

        Debug.Log("EntryPage activated, category buttons hidden, back button visible");
    }



    public void BackToMenu()
    {
        entryPage.SetActive(false);
        menuPage.SetActive(true);

        foreach (Transform child in categoryButtonsContainer.transform)
        {
            child.gameObject.SetActive(true);
        }
        entryBackButton.SetActive(false); // just in case

        selectedCategoryTitle.text = currentCategoryName;
        Debug.Log("Returned to menu, category buttons visible");
    }


    void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            // Clean up listeners before destroying
            var toggle = child.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }

            var buttons = child.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.onClick.RemoveAllListeners();
            }

            Destroy(child.gameObject);
        }
    }

}
