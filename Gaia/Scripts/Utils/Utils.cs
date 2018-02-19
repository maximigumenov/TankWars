using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gaia
{
    public class Utils : MonoBehaviour
    {
        #region Asset directory helpers
        /// <summary>
        /// Get raw gaia asset directory
        /// </summary>
        /// <returns>Base gaia directory</returns>
        public static string GetGaiaAssetDirectory()
        {
            string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// Get the asset directory for a particular featiure type
        /// </summary>
        /// <param name="featureType"></param>
        /// <returns>Path of feature type</returns>
        public static string GetGaiaAssetDirectory(Gaia.GaiaConstants.FeatureType featureType)
        {
            string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
            path = Path.Combine(path, featureType.ToString());
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// Get the full asset path for a specific asset type and name
        /// </summary>
        /// <param name="featureType">The type of feature this asset is</param>
        /// <param name="assetName">The file name of the asset</param>
        /// <returns>Fully qualified path of the asset</returns>
        public static string GetGaiaAssetPath(Gaia.GaiaConstants.FeatureType featureType, string assetName)
        {
            string path = GetGaiaAssetDirectory(featureType);
            path = Path.Combine(GetGaiaAssetDirectory(featureType), assetName);
            return path.Replace('\\','/');
        }

        /// <summary>
        /// Get the full asset path for a specific asset type and name
        /// </summary>
        /// <param name="featureType">The type of feature this asset is</param>
        /// <param name="assetName">The file name of the asset</param>
        /// <returns>Fully qualified path of the asset</returns>
        public static string GetGaiaStampAssetPath(Gaia.GaiaConstants.FeatureType featureType, string assetName)
        {
            string path = GetGaiaAssetDirectory(featureType);
            path = Path.Combine(GetGaiaAssetDirectory(featureType), "Data");
            path = Path.Combine(path, assetName);
            return path.Replace('\\', '/');
        }


        /// <summary>
        /// Parse a stamp preview texture to work out where the stamp lives
        /// </summary>
        /// <param name="source">Source texture</param>
        /// <returns></returns>
        public static string GetGaiaStampPath(Texture2D source)
        {
            string path = "";
            #if UNITY_EDITOR
            path = UnityEditor.AssetDatabase.GetAssetPath(source);
            #endif

            string fileName = Path.GetFileName(path);
            path = Path.Combine(Path.GetDirectoryName(path), "Data");
            path = Path.Combine(path, fileName);
            path = Path.ChangeExtension(path, ".bytes");
            path = GetGaiaAssetDirectory() + path.Replace(Gaia.GaiaConstants.AssetDirFromAssetDB, "");
            path = path.Replace('\\', '/');
            return path;
        }

        /// <summary>
        /// Check to see if this actually a valid stamp - needs a .jpg and a .bytes file
        /// </summary>
        /// <param name="source">Source texture</param>
        /// <returns></returns>
        public static bool CheckValidGaiaStampPath(Texture2D source)
        {
            string path = "";
            #if UNITY_EDITOR
                path = UnityEditor.AssetDatabase.GetAssetPath(source);
            #endif

            path = GetGaiaAssetDirectory() + path.Replace(Gaia.GaiaConstants.AssetDirFromAssetDB, "");

            // Check to see if we have a jpg file
            if (Path.GetExtension(path).ToLower() != ".jpg")
            {
                return false;
            }

            //Check to see if we have asset file
            string fileName = Path.GetFileName(path);
            path = Path.Combine(Path.GetDirectoryName(path), "Data");
            path = Path.Combine(path, fileName);
            path = Path.ChangeExtension(path, ".bytes");
            path = path.Replace('\\', '/');

            if (System.IO.File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Create all the Gaia asset directories for scans to go into
        /// </summary>
        public static void CreateGaiaAssetDirectories()
        {
            string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
            bool addedDir = false;
            try
            {
                foreach (Gaia.GaiaConstants.FeatureType feature in Enum.GetValues(typeof(Gaia.GaiaConstants.FeatureType)))
                {
                    path = GetGaiaAssetDirectory(feature);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        path = Path.Combine(path, "Data");
                        Directory.CreateDirectory(path);
                        addedDir = true;
                    }
                }

                #if UNITY_EDITOR
                if (addedDir)
                {
                    AssetDatabase.Refresh();
                }
                #endif
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to create directory {0} : {1}",path, e.Message));
            }
        }

        /// <summary>
        /// Get all objects of the given type at the location in the path. Only works in the editor.
        /// </summary>
        /// <typeparam name="T">Type of object to load</typeparam>
        /// <param name="path">The path to look in</param>
        /// <returns>List of those objects</returns>
        public static T[] GetAtPath<T>(string path)
        {

            ArrayList al = new ArrayList();

            #if UNITY_EDITOR

            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
            foreach (string fileName in fileEntries)
            {
                int index = fileName.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                    localPath += fileName.Substring(index);

                UnityEngine.Object t = UnityEditor.AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

            #endif

            T[] result = new T[al.Count];
            for (int i = 0; i < al.Count; i++)
                result[i] = (T)al[i];

            return result;
        }

        #endregion

        #region Image helpers



        //TODO - Use asset database methods
        //string meshName = "Assets/UCLAGameLab/Meshes/" + saveName + ".asset";
		//AssetDatabase.CreateAsset(msh, meshName);

        public static void MakeTextureReadable(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
            #if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                if (tImporter.textureType != TextureImporterType.Default || tImporter.isReadable != true)
                {
                    tImporter.textureType = TextureImporterType.Default;
                    tImporter.isReadable = true;
                    AssetDatabase.ImportAsset(assetPath);
                    AssetDatabase.Refresh();
                }
            }
            #endif
        }

        /// <summary>
        /// Compress / encode a single layer map file to an image
        /// </summary>
        /// <param name="input">Single layer map in format x,y</param>
        /// <param name="imageName">Output image name - image image index and extension will be added</param>
        /// <param name="exportPNG">True if a png is wanted</param>
        /// <param name="exportJPG">True if a jpg is wanted</param>
        public static void CompressToSingleChannelFileImage(float[,] input, string imageName, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool exportPNG = true, bool exportJPG = true)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);

            Texture2D exportTexture = new Texture2D(width, height, imageStorageFormat, false);
            Color pixelColor = new Color();
            pixelColor.a = 1f;
            pixelColor.r = pixelColor.g = pixelColor.b = 0f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixelColor.r = pixelColor.b = pixelColor.g = input[x, y];
                    exportTexture.SetPixel(x, y, pixelColor);
                }
            }

            exportTexture.Apply();

            // Write JPG
            if (exportJPG)
            {
                ExportJPG(imageName, exportTexture);
            }

            // Write PNG
            if (exportPNG)
            {
                ExportPNG(imageName, exportTexture);
            }

            //Lose the texture
            DestroyImmediate(exportTexture);
        }

        /// <summary>
        /// Convert the supplied texture to an array based on grayscale value
        /// </summary>
        /// <param name="texture">Input texture - must be read enabled</param>
        /// <returns>Texture as grayscale array</returns>
        public static float[,] ConvertTextureToArray(Texture2D texture)
        {
            float[,] array = new float[texture.width, texture.height];
            for (int x = 0; x < texture.width; x++)
            {
                for (int z = 0; z < texture.height; z++)
                {
                    array[x, z] = texture.GetPixel(x, z).grayscale;
                }
            }
            return array;
        }


        /// <summary>
        /// Decompress a single channel from the provided file into a float array.
        /// </summary>
        /// <param name="fileName">File to process</param>
        /// <param name="channelR">Take data from R channel</param>
        /// <param name="channelG">Take data from G channel</param>
        /// <param name="channelB">Take data from B channel</param>
        /// <param name="channelA">Take data from A channel</param>
        /// <returns>Array of float values from the selected channel</returns>
        public static float[,] DecompressFromSingleChannelFileImage(string fileName, int width, int height, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool channelR = true, bool channelG = false, bool channelB = false, bool channelA = false)
        {
            float[,] retArray = null;

            if (System.IO.File.Exists(fileName))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(fileName);
                Texture2D importTexture = new Texture2D(width, height, imageStorageFormat, false);
                importTexture.LoadImage(bytes);
                retArray = new float[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).r;
                    }
                }
                //Lose the texture
                DestroyImmediate(importTexture);
            }
            else
            {
                Debug.LogError("Unable to find " + fileName);
            }
            return retArray;
        }

        /// <summary>
        /// Decompress a single channel from the provided file into a float array.
        /// </summary>
        /// <param name="fileName">File to process</param>
        /// <param name="channelR">Take data from R channel</param>
        /// <param name="channelG">Take data from G channel</param>
        /// <param name="channelB">Take data from B channel</param>
        /// <param name="channelA">Take data from A channel</param>
        /// <returns>Array of float values from the selected channel</returns>
        public static float[,] DecompressFromSingleChannelTexture(Texture2D importTexture, bool channelR = true, bool channelG = false, bool channelB = false, bool channelA = false)
        {
            if ((importTexture == null) || importTexture.width <= 0 || importTexture.height <= 0)
            {
                Debug.LogError("Unable to import from texture");
                return null;
            }

            float[,] retArray = new float[importTexture.width, importTexture.height];

            if (channelR)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).r;
                    }
                }
            }
            else if (channelG)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).g;
                    }
                }
            }
            else if (channelB)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).b;
                    }
                }
            }
            if (channelA)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).a;
                    }
                }
            }
            return retArray;
        }

        /// <summary>
        /// Export a texture to jpg
        /// </summary>
        /// <param name="fileName">File name to us - will have .jpg appended</param>
        /// <param name="texture">Texture source</param>
        public static void ExportJPG(string fileName, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToJPG();
            File.WriteAllBytes(fileName + ".jpg", bytes);
        }

        /// <summary>
        /// Export a texture to png
        /// </summary>
        /// <param name="fileName">File name to us - will have .png appended</param>
        /// <param name="texture">Texture source</param>
        public static void ExportPNG(string fileName, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(fileName + ".png", bytes);
        }

        /// <summary>
        /// Will import the raw file provided - it assumes that it is in a square 16 bit PC format
        /// </summary>
        /// <param name="fileName">Fully qualified file name</param>
        /// <returns>File contents or null</returns>
        public static float[,] LoadRawFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("Could not locate heightmap file : " + fileName);
                return null;
            }

            float[,] heights = null;
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    int mapSize = Mathf.CeilToInt(Mathf.Sqrt(fileStream.Length / 2));
                    heights = new float[mapSize, mapSize];
                    for (int x = 0; x < mapSize; x++)
                    {
                        for (int y = 0; y < mapSize; y++)
                        {
                            heights[x, y] = (float)(br.ReadUInt16() / 65535.0f);
                        }
                    }
                }
                fileStream.Close();
            }

            return heights;
        }


        #endregion

        #region Mesh helpers

        /// <summary>
        /// Create a mesh for the heightmap
        /// </summary>
        /// <param name="heightmap"></param>
        /// <param name="targetSize"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static Mesh CreateMesh(float[,] heightmap, Vector3 targetSize)
        {
            //Need to sample these to not blow unity mesh sizes
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            int targetRes = 1;

            Vector3 targetOffset = Vector3.zero - (targetSize / 2f);
            Vector2 uvScale = new Vector2(1.0f / (width - 1), 1.0f / (height - 1));

            //Choose best possible target res
            for (targetRes = 1; targetRes < 100; targetRes++ )
            {
                if ( ( (width / targetRes) * (height / targetRes) ) < 65000 )
                {
                    break;
                }
            }

            targetSize = new Vector3(targetSize.x / (width - 1) * targetRes, targetSize.y, targetSize.z / (height - 1) * targetRes);
            width = (width - 1) / targetRes + 1;
            height = (height - 1) / targetRes + 1;

            Vector3[] vertices = new Vector3[width * height];
            Vector2[] uvs = new Vector2[width * height];
            Vector3[] normals = new Vector3[width * height];
            Color[] colors = new Color[width * height];
            int[] triangles = new int[(width - 1) * (height - 1) * 6];

            // Build vertices and UVs
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[y * width + x] = Color.black;
                    normals[y * width + x] = Vector3.up;
                    //vertices[y * w + x] = Vector3.Scale(targetSize, new Vector3(-y, heightmap[x * tRes, y * tRes], x)) + targetOffset;
                    vertices[y * width + x] = Vector3.Scale(targetSize, new Vector3(x, heightmap[x * targetRes, y * targetRes], y)) + targetOffset;
                    uvs[y * width + x] = Vector2.Scale(new Vector2(x * targetRes, y * targetRes), uvScale);
                }
            }

            // Build triangle indices: 3 indices into vertex array for each triangle
            int index = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    triangles[index++] = (y * width) + x;
                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = (y * width) + x + 1;
                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = ((y + 1) * width) + x + 1;
                    triangles[index++] = (y * width) + x + 1;
                }
            }

            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            ;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }
        #endregion

        #region Direction helpers

        /// <summary>
        /// Rotate a direction vector left 90% around X axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftXAxis(Vector3 input)
        {
            return new Vector3(input.x, -input.z, input.y);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around X axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightXAxis(Vector3 input)
        {
            return new Vector3(input.x, input.z, -input.y);
        }

        /// <summary>
        /// Rotate a direction vector left 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftYAxis(Vector3 input)
        {
            return new Vector3(-input.z, input.y, input.x);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightYAxis(Vector3 input)
        {
            return new Vector3(input.z, input.y, -input.x);
        }

        /// <summary>
        /// Rotate a direction vector left 90% around Z axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftZAxis(Vector3 input)
        {
            return new Vector3(input.y, -input.x, input.z);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightZAxis(Vector3 input)
        {
            return new Vector3(-input.y, input.x, input.z);
        }

        #endregion

        #region Math helpers

        /// <summary>
        /// Return true if the values are approximately equal
        /// </summary>
        /// <param name="a">Parameter A</param>
        /// <param name="b">Parameter B</param>
        /// <param name="threshold">Threshold to test for</param>
        /// <returns>True if approximately equal</returns>
        public static bool Math_ApproximatelyEqual(float a, float b, float threshold)
        {
            if (Mathf.Abs(a - b) < threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if the value is a power of 2
        /// </summary>
        /// <param name="value">Value to be checked</param>
        /// <returns>True if a power of 2</returns>
        public static bool Math_IsPowerOf2(int value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Returned value clamped in range of min to max
        /// </summary>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="value">Value to check</param>
        /// <returns>Clamped value</returns>
        public static float Math_Clamp(float min, float max, float value)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        /// <summary>
        /// Return mod of value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="mod">Mod value</param>
        /// <returns>Mode of value</returns>
        public static float Math_Modulo(float value, float mod)
        {
            return value - mod * (float)Math.Floor(value / mod);
        }

        /// <summary>
        /// Return mod of value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="mod">Mod value</param>
        /// <returns>Mode of value</returns>
        public static int Math_Modulo(int value, int mod)
        {
            return (int)(value - mod * (float)Math.Floor((float)value / mod));
        }

        /// <summary>
        /// Linear interpolation between two values
        /// </summary>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <param name="fraction">Fraction</param>
        /// <returns></returns>
        public static float Math_InterpolateLinear(float value1, float value2, float fraction)
        {
            return value1 * (1f - fraction) + value2 * fraction;
        }

        /// <summary>
        /// Smooth interpolation between two values
        /// </summary>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <param name="fraction">Fraction</param>
        /// <returns></returns>
        public static float Math_InterpolateSmooth(float value1, float value2, float fraction)
        {
            if (fraction < 0.5f)
            {
                fraction = 2f * fraction * fraction;
            }
            else
            {
                fraction = 1f - 2f * (fraction - 1f) * (fraction - 1f);
            }
            return value1 * (1f - fraction) + value2 * fraction;
        }

        /// <summary>
        /// Calculate the distance between two points
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <returns></returns>
        public static float Math_Distance(float x1, float y1, float x2, float y2)
        {
            return Mathf.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }

        public static float Math_InterpolateSmooth2(float v1, float v2, float fraction)
        {
            float fraction2 = fraction * fraction;
            fraction = 3 * fraction2 - 2f * fraction * fraction2;
            return v1 * (1f - fraction) + v2 * fraction;
        }

        public static float Math_InterpolateCubic(float v0, float v1, float v2, float v3, float fraction)
        {
            float p = (v3 - v2) - (v0 - v1);
            float q = (v0 - v1) - p;
            float r = v2 - v0;
            float fraction2 = fraction * fraction;
            return p * fraction * fraction2 + q * fraction2 + r * fraction + v1;
        }

        #endregion

    }
}