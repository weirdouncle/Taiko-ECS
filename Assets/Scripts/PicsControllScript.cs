using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using NAudio.Gui;

public class PicsControllScript : MonoBehaviour
{
    [SerializeField] private Material material;
    public static List<KeyValuePair<Texture2D, Vector2>> SeNotes = new();
    public static List<BatchMaterialID> BatchMaterials = new();
    public static List<Sprite> SeSprite=new();
    void Start()
    {
        LoadPics();
        RegisterMats().Forget();
    }

    private async UniTaskVoid RegisterMats()
    {
        await UniTask.WaitUntil(() => World.DefaultGameObjectInjectionWorld?.GetExistingSystemManaged<EntitiesGraphicsSystem>() != null);

        World world = World.DefaultGameObjectInjectionWorld;
        var system = world.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();

        foreach (KeyValuePair <Texture2D, Vector2> key in SeNotes)
        {
            Texture2D texture = key.Key;
            Material mat = new Material(material);
            mat.SetTexture("_MainTex", texture);

            Vector2 vector = new(0.5f - key.Value.x, 0.5f- key.Value.y);
            mat.SetVector("_Align", vector);

            BatchMaterialID id = system.RegisterMaterial(mat);
            BatchMaterials.Add(id);
        }
    }

    public void LoadPics()
    {
        List<Sprite> sprites = new List<Sprite>(Resources.LoadAll<Sprite>("Texture/localization"));
        string suffix = "_jp";
        List<Sprite> se_sprites = new List<Sprite>(Resources.LoadAll<Sprite>("Texture/senotes"));
        //don
        for (int i = 1; i <= 4; i++)
        {
            Sprite don = se_sprites.Find(t => t.name == string.Format("don{0}{1}", i, suffix));
            if (don == null) don = se_sprites.Find(t => t.name == string.Format("don{0}_en", i));
            SeNotes.Add(SpriteToTexture(don));
            SeSprite.Add(don);
        }
        //ka
        for (int i = 1; i <= 3; i++)
        {
            Sprite ka = se_sprites.Find(t => t.name == string.Format("ka{0}{1}", i, suffix));
            if (ka == null) ka = se_sprites.Find(t => t.name == string.Format("ka{0}_en", i));
            SeNotes.Add(SpriteToTexture(ka));
            SeSprite.Add(ka);
        }
        //rapid
        for (int i = 1; i <= 2; i++)
        {
            Sprite ka = se_sprites.Find(t => t.name == string.Format("roll{0}{1}", i, suffix));
            if (ka == null) ka = se_sprites.Find(t => t.name == string.Format("roll{0}_en", i));
            SeNotes.Add(SpriteToTexture(ka));
            SeSprite.Add(ka);
        }
        //9
        Sprite body = se_sprites.Find(t => t.name == string.Format("roll_body{0}", suffix));
        if (body == null) body = se_sprites.Find(t => t.name == "roll_body_en");
        SeNotes.Add(SpriteToTexture(body));
        SeSprite.Add(body);
        //10
        Sprite tail = se_sprites.Find(t => t.name == string.Format("roll_tail{0}", suffix));
        if (tail == null) tail = se_sprites.Find(t => t.name == "roll_tail_en");
        SeNotes.Add(SpriteToTexture(tail));
        SeSprite.Add(tail);
        //11
        Sprite balloon = se_sprites.Find(t => t.name == string.Format("balloon{0}", suffix));
        if (balloon == null) balloon = se_sprites.Find(t => t.name == "balloon_en");
        SeNotes.Add(SpriteToTexture(balloon));
        SeSprite.Add(balloon);
        //12
        Sprite hammer = se_sprites.Find(t => t.name == string.Format("hammer{0}", suffix));
        if (hammer == null) hammer = se_sprites.Find(t => t.name == "hammer_en");
        SeNotes.Add(SpriteToTexture(hammer));
        SeSprite.Add(hammer);

        //kusudama 13
        Sprite kusudama = se_sprites.Find(t => t.name == string.Format("kusudama{0}", suffix));
        if (kusudama == null) kusudama = se_sprites.Find(t => t.name == "kusudama_en");
        SeNotes.Add(SpriteToTexture(kusudama));
        SeSprite.Add(hammer);

        //hands 14/15
        SeNotes.Add(SpriteToTexture(se_sprites.Find(t => t.name == string.Format("don5{0}", suffix))));
        SeNotes.Add(SpriteToTexture(se_sprites.Find(t => t.name == string.Format("ka4{0}", suffix))));
        SeSprite.Add(se_sprites.Find(t => t.name == string.Format("don5{0}", suffix)));
        SeSprite.Add(se_sprites.Find(t => t.name == string.Format("ka4{0}", suffix)));
    }

    private KeyValuePair<Texture2D, Vector2> SpriteToTexture(Sprite sprite)
    {
        // 获取 Sprite 绑定的 Texture2D
        Texture2D originalTexture = sprite.texture;
        // 获取 Sprite 在 Texture2D 上的 UV 位置
        Rect spriteRect = sprite.rect;

        // 计算裁剪区域
        int width = (int)spriteRect.width;
        int height = (int)spriteRect.height;
        Texture2D newTexture = new Texture2D(width, height);

        // 读取像素数据
        Color[] pixels = originalTexture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            width,
            height
        );

        // 创建新的 Texture2D
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return new KeyValuePair<Texture2D, Vector2>(newTexture, sprite.pivot / sprite.rect.size);
    }
}
