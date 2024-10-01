/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenShot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeScreenshot(string fullpath)
    {
        if (GetComponent<Camera>() == null)
        {
            camera = GetComponent<Camera>();
        }

        RenderTexture rt = new RenderTexture(256, 256, 24);
        GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0,0);
        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullpath, bytes);
//#if UNITY_EDITOR
//        AssetDatabase.Refresh();
//#endif
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TakeScreenshot : MonoBehaviour
{
    Camera cam;
    public string pathFolder;

    public List<GameObject> sceneObjects;
    //public List<InventoryItemData> dataObjects;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    [ContextMenu("Screenshot")]
    private void ProcessScreenshots()
    {
        StartCoroutine(Screenshot());
    }

    private IEnumerator Screenshot()
    {
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            GameObject obj = sceneObjects[i];
            //InventoryItemData data = dataObjects[i];

            obj.gameObject.SetActive(true);
            yield return null;

            TakeShot($"{Application.dataPath}/{pathFolder}/Icon.png");

            yield return null;
            obj.gameObject.SetActive(false);

            #if UNITY_EDITOR
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{pathFolder}/Icon.png");
            if(s!= null)
            {
                //data.icon = s;
                //EditorUtility.SetDirty(data);
            }
            #endif

            yield return null;
        }
    }

    public void TakeShot(string fullPath)
    {
        if(cam == null)
        {
            cam = GetComponent<Camera>();
        }

        RenderTexture rt = new RenderTexture(256, 256, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);

        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }
}
