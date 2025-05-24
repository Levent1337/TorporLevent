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
        foreach (var category in codexManager.codexData.categories)
        {
            // Create a local copy of the category to avoid closure issues in the listener
            var localCategory = category;

            GameObject button = Instantiate(categoryButtonPrefab, categoryContainer);
            button.GetComponentInChildren<TMP_Text>().text = localCategory.name;

            // Assign the onClick handler using the local copy
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentCategoryName = localCategory.name;
                selectedCategoryTitle.text = localCategory.name;
                Debug.Log("Category clicked: " + localCategory.name);
                GenerateTopics(localCategory);
            });
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
        entryTitle.text = entry.title;
        entryDescription.text = entry.description;

        if (!string.IsNullOrEmpty(entry.image))
        {
            Sprite loaded = Resources.Load<Sprite>("Images/" + entry.image);
            entryImage.sprite = loaded != null ? loaded : defaultImage;
        }
        else
        {
            entryImage.sprite = defaultImage;
        }

        // UI switch
        selectedCategoryTitle.text = entry.title;
        menuPage.SetActive(false);
        entryPage.SetActive(true);
        categoryButtonsContainer.SetActive(false);
        entryBackButton.SetActive(true);
    }

    public void BackToMenu()
    {
        entryPage.SetActive(false);
        menuPage.SetActive(true);
        categoryButtonsContainer.SetActive(true);
        entryBackButton.SetActive(false);
        selectedCategoryTitle.text = currentCategoryName;
    }

    void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
