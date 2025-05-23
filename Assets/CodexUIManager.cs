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
            button.GetComponentInChildren<Text>().text = category.name;
            button.GetComponent<Button>().onClick.AddListener(() => GenerateTopics(category));
        }
    }

    void GenerateTopics(Category category)
    {
        ClearContainer(topicContainer);
        ClearContainer(entryContainer);

        foreach (var topic in category.topics)
        {
            GameObject button = Instantiate(topicButtonPrefab, topicContainer);
            button.GetComponentInChildren<Text>().text = topic.name;
            button.GetComponent<Button>().onClick.AddListener(() => GenerateEntries(topic));
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
