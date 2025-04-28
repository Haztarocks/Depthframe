using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

namespace CharacterEditor2D
{
    public static class SpriteSlicerUtils
    {
        public static List<Sprite> SliceSprite(string sourcePath, string targetPath, params string[] excludedName)
        {
            try
            {
                TextureImporter sourceti = (TextureImporter)AssetImporter.GetAtPath(sourcePath);
                TextureImporter targetti = (TextureImporter)AssetImporter.GetAtPath(targetPath);

                // Reset target texture importer
                targetti.spriteImportMode = SpriteImportMode.Single;
                targetti.SaveAndReimport();

                // Set sprite import mode
                targetti.spriteImportMode = sourceti.spriteImportMode;

                // Access sprite metadata using SpriteDataProvider
                var sourceProvider = GetSpriteEditorDataProvider(sourceti);
                var sourceSprites = sourceProvider.GetSpriteRects();

                var targetProvider = GetSpriteEditorDataProvider(targetti);
                var targetSprites = new List<SpriteRect>();

                foreach (var spriteRect in sourceSprites)
                {
                    if (contains(spriteRect.name, excludedName))
                        continue;

                    var newSpriteRect = new SpriteRect
                    {
                        alignment = spriteRect.alignment,
                        border = spriteRect.border,
                        name = spriteRect.name,
                        pivot = spriteRect.pivot,
                        rect = spriteRect.rect
                    };
                    targetSprites.Add(newSpriteRect);
                }

                targetProvider.SetSpriteRects(targetSprites.ToArray());
                targetti.SaveAndReimport();

                Object[] tobj = AssetDatabase.LoadAllAssetsAtPath(targetPath);
                List<Sprite> val = new List<Sprite>();
                foreach (Object o in tobj)
                {
                    if (o is Sprite)
                        val.Add((Sprite)o);
                }

                return val;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return null;
            }
        }

        public static List<string> GetSlicedNames(Texture2D texture)
        {
            if (texture == null)
                return new List<string>();

            List<string> val = new List<string>();
            TextureImporter tempti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));

            // Access sprite metadata using SpriteDataProvider
            var provider = GetSpriteEditorDataProvider(tempti);
            var sprites = provider.GetSpriteRects();

            foreach (var spriteRect in sprites)
                val.Add(spriteRect.name);

            return val;
        }

        private static ISpriteEditorDataProvider GetSpriteEditorDataProvider(TextureImporter importer)
        {
            return AssetImporter.GetAtPath(importer.assetPath) as ISpriteEditorDataProvider;
        }

        private static bool contains(string value, string[] listVal)
        {
            foreach (string v in listVal)
            {
                if (value == v)
                    return true;
            }

            return false;
        }
    }
}