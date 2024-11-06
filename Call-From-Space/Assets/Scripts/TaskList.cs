using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour
{

    public struct TaskData
    {
        public string text;
        public bool isLargeText;
        public TaskData(string _text, bool _isLargeText)
        {
            text = _text;
            isLargeText = _isLargeText;
        }
    }

    GameObject TaskContainer;
    GameObject TaskTemplate;
    public List<TaskData> tasks;
    // Start is called before the first frame update
    void Start()
    {
        TaskContainer = transform.Find("TaskContainer").gameObject;
        TaskTemplate = TaskContainer.transform.Find("TaskTemplate").gameObject;

        tasks = new List<TaskData>();
        AddTask("Find Your Way Into The Station", true);
    }

    // Update is called once per frame
    public void AddTask(string _text, bool _isLargeText)
    {
        TaskData task = new TaskData(_text, _isLargeText);
        tasks.Add(task);
        if(TaskContainer.activeSelf)
            refresh();
    }

    public void DeleteTask(string _text)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].text == _text)
            {
                tasks.RemoveAt(i);
                if(TaskContainer.activeSelf)
                    refresh();
                return;
            }
        }
    }


    public void refresh()
    {
        foreach( Transform child in TaskContainer.transform)
        {
            if (child == TaskTemplate.transform) continue;
            Destroy(child.gameObject);
        }
        int ypos = 0;
        //45 for small task and 60 for large task
        float distance = 45f;

        foreach (TaskData task in tasks)
        {
            //Debug.Log(task);
            RectTransform TaskRectTransform = Instantiate(TaskTemplate.transform, TaskContainer.transform).GetComponent<RectTransform>();
            TaskRectTransform.gameObject.SetActive(true);
            Vector2 additionalPosition = new Vector2(0, ypos*distance);
            TaskRectTransform.anchoredPosition = additionalPosition;
            TaskRectTransform.Find("TaskText").GetComponent<TMPro.TextMeshProUGUI>().text = task.text;
            ypos -= 1;

            if(task.isLargeText)
                distance = 60f;
            else
                distance = 45f;
        }
    }
}
