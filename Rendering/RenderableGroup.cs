using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Instantiates and updates game objects based on a list of renderables.
public class RenderableGroup<RenderableType>
{
    private List<GameObject> mDividerPool = new List<GameObject>();
    private List<GameObject> mObjectPool = new List<GameObject>();
    private Transform mContainer;
    private GameObject mNewObjectInstance;
    private System.Action<RenderableType, GameObject> mRenderFunction;
    private System.Action<GameObject> mOnCreateCallback;

    private GameObject mDividerObject;

    // Creates a renderable group based on a container object,
    // assuming that the container has exactly one child that should be used as a template for new-object instance
    // In the case of UI, the container can have a layout group to position the elements.
    public RenderableGroup(
        Transform container,
        System.Action<RenderableType, GameObject> renderFunction,
        System.Action<GameObject> onCreateCallback = null)
    {
        mContainer = container;
        // Disable and detach the child, we just want to be able to copy it later
        GameObject child = container.GetChild(0).gameObject;
        child.SetActive(false);
        child.transform.SetParent(null, false);
        // The child game object will act as a template for the renderables in this group.
        mNewObjectInstance = child;
        mRenderFunction = renderFunction;
        mOnCreateCallback = onCreateCallback;
        InitializeDividerObject();
    }

    // Should be called once per frame
    public void UpdateRenderables(List<RenderableType> renderables)
    {
        // No dividers
        UpdateRenderables(renderables, new int[] { });
    }

    // Should be called once per frame
    public void UpdateRenderables(List<RenderableType> renderables, int[] dividerIndexes)
    {
        // First, remove all the old dividers
        for (int i = mContainer.childCount - 1; i >= 0; i--)
        {
            GameObject obj = mContainer.GetChild(i).gameObject;
            if (IsDivider(obj))
            {
                mDividerPool.Add(obj);
                obj.SetActive(false);
                obj.transform.SetParent(null, false);
            }
        }
        // Then update the renderables
        int index = 0;
        if (renderables != null)
        {
            foreach (RenderableType renderable in renderables)
            {
                GameObject renderObject;
                // Spawn new renderable
                if (index >= mContainer.childCount)
                {
                    if (mObjectPool.Count > 0)
                    {
                        renderObject = mObjectPool[mObjectPool.Count - 1];
                        mObjectPool.RemoveAt(mObjectPool.Count - 1);
                    }
                    else
                    {
                        renderObject = GameObject.Instantiate(mNewObjectInstance);
                    }
                    renderObject.transform.SetParent(mContainer, false);
                    if (mOnCreateCallback != null)
                    {
                        mOnCreateCallback(renderObject);
                    }
                }
                else
                {
                    renderObject = mContainer.GetChild(index).gameObject;
                }
                renderObject.SetActive(true);
                mRenderFunction(renderable, renderObject);
                index++;
            }
        }
        // Disable any excess children
        for (int i = index; i < mContainer.childCount; i++)
        {
            GameObject excessObject = mContainer.GetChild(i).gameObject;
            mObjectPool.Add(excessObject);
            excessObject.SetActive(false);
            excessObject.transform.SetParent(null, false);
        }
        // Then add new dividers
        for (int j = 0; j < dividerIndexes.Length; j++)
        {
            GameObject divider;
            if (mDividerPool.Count > 0)
            {
                divider = mDividerPool[mDividerPool.Count - 1];
                mDividerPool.RemoveAt(mDividerPool.Count - 1);
            }
            else
            {
                divider = GameObject.Instantiate(mDividerObject);
            }
            divider.transform.SetParent(mContainer, false);
            divider.SetActive(true);
            divider.transform.SetSiblingIndex(dividerIndexes[j]);
        }
    }

    private void InitializeDividerObject()
    {
        mDividerObject = new GameObject("Divider");
        RectTransform rect = mDividerObject.AddComponent<RectTransform>();
        // dividers should be right-aligned
        // anchors should all be 0.5
        rect.sizeDelta = new Vector2(600f, 2f);
        Image img = mDividerObject.AddComponent<Image>();
        img.color = Color.black;
    }

    private bool IsDivider(GameObject obj)
    {
        return obj.name.Contains("Divider");
    }
}