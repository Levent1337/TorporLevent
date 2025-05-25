using System;
using System.Collections.Generic;

[Serializable]
public class CodexRoot
{
    public List<Category> categories;
}

[Serializable]
public class Category
{
    public string name;
    public List<Topic> topics;
}

[Serializable]
public class Topic
{
    public string name;
    public List<Entry> entries;
}

[Serializable]
public class Entry
{
    public string title;
    public string description;
    public string image;
}
