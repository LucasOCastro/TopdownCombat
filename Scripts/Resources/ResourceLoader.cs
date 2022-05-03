using Godot;

namespace CombatGame
{
    public static class ResourceLoader
    {
        //TODO add abstraction
        private const string resourcesPath = "res://Resources/";
        private const string terrainResourcesPath = resourcesPath + "Terrain";
        private const string entityBasesResourcesPath = resourcesPath + "Entities";
        private const string weaponsResourcesPath = resourcesPath + "Items/Weapons";
        private const string structuresResourcePath = resourcesPath + "Structures";

        public static void LoadAll()
        {
            LoadAllInDir<Terrain>(terrainResourcesPath, ResourceDatabase<Terrain>.Register);
            for (int i = 0; i < ResourceDatabase<Terrain>.AllResources.Count; i++)
            {
                ResourceDatabase<Terrain>.AllResources[i].terrainIndex = i;
            }

            LoadAllInDir<EntityBase>(entityBasesResourcesPath, ResourceDatabase<EntityBase>.Register);
            LoadAllInDir<WeaponBase>(weaponsResourcesPath, ResourceDatabase<WeaponBase>.Register);
            LoadAllInDir<StructureBase>(structuresResourcePath, ResourceDatabase<StructureBase>.Register);
        }

        public static ImageTexture GenTerrainsTexture()
        {
            //TODO hardcoded values
            Image image = new Image();
            int terrainCount = ResourceDatabase<Terrain>.AllResources.Count;
            image.Create(terrainCount*32,32,false,Image.Format.Rgb8);
            for (int i = 0; i < terrainCount; i++)
            {
                Terrain terrain = ResourceDatabase<Terrain>.AllResources[i];
                image.BlitRect(terrain.Texture.GetData(), new Rect2(Vector2.Zero, terrain.Texture.GetSize()), new Vector2(i*32, 0));
            }
            
            ImageTexture tex = new ImageTexture();
            tex.CreateFromImage(image, 0);
            return tex;
        }

        public static TextureArray GenTerrainsTextureArray()
        {
            //TODO hardcoded values
            TextureArray array = new TextureArray();
            int terrainCount = ResourceDatabase<Terrain>.AllResources.Count;
            array.Create(32, 32, (uint)terrainCount, Image.Format.Rgb8);
            for (int i = 0; i < terrainCount; i++)
            {
                Terrain terrain = ResourceDatabase<Terrain>.AllResources[i];
                array.SetLayerData(terrain.Texture.GetData(), i);
            }
            return array;
        }
        

        private static void LoadAllInDir<T>(string path, System.Action<T> registerAction) where T: Resource
        {
            Directory dir = new Directory();
            dir.Open(path);
            dir.ListDirBegin();
            for (string file = dir.GetNext(); file != ""; file = dir.GetNext())
            {
                if (dir.CurrentIsDir())
                {
                    //LoadInDir<T>(file, registerAction);
                    continue;
                }

                T resource = GD.Load(path + "/" + file) as T;
                if (resource != null)
                {
                    registerAction(resource);
                }

            }
            dir.ListDirEnd();
        }
    }
}