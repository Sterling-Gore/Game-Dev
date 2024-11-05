using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{

    Transform TaskContainer;
    Transform TaskTemplate;
    public List<string> tasks;
    // Start is called before the first frame update
    void Start()
    {
        TaskContainer = transform.Find("TaskContainer");
        TaskTemplate = transform.Find("TaskTemplate");
        tasks = new List<string>();
        tasks.Add("Hello");
        tasks.Add("Hello");
        tasks.Add("Hello");
        tasks.Add("Hello");
        tasks.Add("Hello");
        refresh();
    }

    // Update is called once per frame
    public void AddTask(string task)
    {
        tasks.Add(task);
    }


    void refresh()
    {
        foreach( Transform child in TaskContainer)
        {
            if (child == TaskTemplate) continue;
            Destroy(child.gameObject);
        }
        int ypos = 0;

        foreach(string task in tasks)
        {
            RectTransform TaskRectTransform = Instantiate(TaskTemplate, TaskContainer).GetComponent<RectTransform>();
            TaskRectTransform.gameObject.SetActive(true);
            Vector2 additionalPosition = new Vector2(0, ypos*75f);
            TaskRectTransform.anchoredPosition = additionalPosition;

            ypos += 1;
        }
    }
}
