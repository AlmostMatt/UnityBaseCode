using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityBaseCode
{
    namespace Rendering
    {
        public class SpriteDictionary : MonoBehaviour
        {
            [System.Serializable]
            public struct NamedSprite
            {
                public string name;
                public Sprite sprite;
            }
            // These fields are functionally equivalent, but they logically group icons
            // If you add a group to this, add a reference to it in Setup
            public NamedSprite[] namedImages1;
            public NamedSprite[] namedImages2;
            public NamedSprite[] namedImages3;

            private Dictionary<string, Sprite> stringToSpriteMap = new Dictionary<string, Sprite>();
            private static SpriteDictionary mSpriteManagerSingleton;

            void Awake()
            {
                if (stringToSpriteMap.Count == 0)
                {
                    Setup();
                }
            }

            private void Setup()
            {
                foreach (NamedSprite[] namedSpriteArray in new NamedSprite[][] {
                    namedImages1,
                    namedImages2,
                    namedImages3,
                })
                {
                    foreach (NamedSprite namedSprite in namedSpriteArray)
                    {
                        if (stringToSpriteMap.ContainsKey(namedSprite.name))
                        {
                            Debug.LogWarning("Multiple sprites with name: " + namedSprite.name);
                        }
                        stringToSpriteMap.Add(namedSprite.name, namedSprite.sprite);
                    }
                }
            }

            public static Sprite GetSprite(string imageName)
            {
                if (mSpriteManagerSingleton == null)
                {
                    mSpriteManagerSingleton = GameObject.FindObjectsOfType<SpriteDictionary>()[0];
                }
                return mSpriteManagerSingleton.GetSpriteInternal(imageName);
            }

            // To be called by the static GetSprite
            public Sprite GetSpriteInternal(string imageName)
            {
                if (imageName == "" || imageName == "None")
                {
                    return null;
                }
                // Call Setup here in case awake hasnt yet happened
                if (stringToSpriteMap.Count == 0)
                {
                    Setup();
                }
                if (stringToSpriteMap.ContainsKey(imageName))
                {
                    return stringToSpriteMap[imageName];
                }
                Debug.LogWarning("WARNING(SpriteManager): No sprite found for name: " + imageName);
                return null;
            }
        }
    }
}