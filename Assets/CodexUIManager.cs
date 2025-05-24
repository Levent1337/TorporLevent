using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CodexUIManager : MonoBehaviour
{
    public CodexManager codexManager;

    public GameObject codexPanel;
    public GameObject menuPage;
    public GameObject entryPage;

    public Transform categoryContainer;
    public Transform topicContainer;
    public Transform entryContainer;

    public GameObject categoryButtonPrefab;
    public GameObject topicButtonPrefab;
    public GameObject entryButtonPrefab;

    public TMP_Text entryTitle;
    public TMP_Text entryDescription;
    public Image entryImage;

    public TMP_Text selectedCategoryTitle;
    public GameObject topicGroupPrefab;

    public Sprite defaultImage;

    void Start()
    {
        codexPanel.SetActive(false);
        entryPage.SetActive(false);
        menuPage.SetActive(true);
        GenerateCategories();
    }

    public void ToggleCodex()
    {
        codexPanel.SetActive(!codexPanel.activeSelf);
    }

    void GenerateCategories()
    {
        foreach (var category in codexManager.codexData.categories)
        {
            GameObject button = Instantiate(categoryButtonPrefab, categoryContainer);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.text = category.name;
            }
            else
            {
                Debug.LogWarning("No TMP_Text found in category button prefab!");
            }

            button.GetComponent<Button>().onClick.AddListener(() => GenerateTopics(category));
        }
    }



    void GenerateTopics(Category category)
    {
        // Clear existing topic groups
        ClearContainer(topicContainer); // topicContainer = Content of ScrollView

        // Update category title at the top (optional)
        if (selectedCategoryTitle != null)
            selectedCategoryTitle.text = category.name;

        foreach (var topic in category.topics)
        {
            // Instantiate the TopicGroup prefab under the topic container
            GameObject topicGroup = Instantiate(topicGroupPrefab, topicContainer);

            // Find the Toggle inside the topic group and set its label
            TMP_Text topicLabel = topicGroup.GetComponentInChildren<TMP_Text>();
            if (topicLabel != null)
                topicLabel.text = topic.name;

            // Find the entry container (EntryButtonsContainer) inside this topic group
            Transform entryContainer = topicGroup.transform.Find("EntryButtonsContainer");

            if (entryContainer == null)
            {
                Debug.LogError("EntryButtonsContainer not found in TopicGroup prefab!");
                continue;
            }

            // Instantiate entry buttons for this topic
            foreach (var entry in topic.entries)
            {
                GameObject entryButton = Instantiate(entryButtonPrefab, entryContainer);
                TMP_Text entryText = entryButton.GetComponentInChildren<TMP_Text>();
                if (entryText != null)
                    entryText.text = entry.title;

                entryButton.GetComponent<Button>().onClick.AddListener(() => ShowEntry(entry));
            }

            // Collapse entry list by default
            entryContainer.gameObject.SetActive(false);

            // Hook toggle to show/hide the entries
            Toggle topicToggle = topicGroup.GetComponentInChildren<Toggle>();
            if (topicToggle != null)
            {
                topicToggle.onValueChanged.AddListener(isOn =>
                {
                    entryContainer.gameObject.SetActive(isOn);
                });
            }
            else
            {
                Debug.LogWarning("Toggle not found in TopicGroup prefab.");
            }
        }
    }

    void GenerateEntries(Topic topic)
    {
        ClearContainer(entryContainer);

        foreach (var entry in topic.entries)
        {
            GameObject button = Instantiate(entryButtonPrefab, entryContainer);
            button.GetComponentInChildren<Text>().text = entry.title;
            button.GetComponent<Button>().onClick.AddListener(() => ShowEntry(entry));
        }
    }

    void ShowEntry(Entry entry)
    {
        entryTitle.text = entry.title;
        entryDescription.text = entry.description;

        if (!string.IsNullOrEmpty(entry.image))
        {
            Sprite img = Resources.Load<Sprite>("Images/" + entry.image);
            entryImage.sprite = img != null ? img : defaultImage;
        }
        else
        {
            entryImage.sprite = defaultImage;
        }

        entryPage.SetActive(true);
        menuPage.SetActive(false);
    }

    public void BackToMenu()
    {
        entryPage.SetActive(false);
        menuPage.SetActive(true);
    }

    void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
