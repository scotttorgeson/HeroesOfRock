using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;
using System.IO;

namespace Pipeline
{
    [ContentProcessor(DisplayName = "Deferred Rendering Model")]
    public class DeferredRendererModel : ModelProcessor
    {
        //Directory
        string directory;

        //Normal Map Texture Property
        [DisplayName("Normal Map Texture")]
        [Description("This will be used as the normal map on the model, if not set will use a default.")]
        [DefaultValue("")]
        public string NormalMapTexture
        {
            get { return normalMapTexture; }
            set { normalMapTexture = value; }
        }
        private string normalMapTexture;

        //Normal Map Key, will be used to search for the normal map in the opaque data of the model
        [DisplayName("Normal Map Key")]
        [Description("This will be the key that will be used to search for the normal map in the opaque data of the model")]
        [DefaultValue("NormalMap")]
        public string NormalMapKey
        {
            get { return normalMapKey; }
            set { normalMapKey = value; }
        }
        private string normalMapKey = "NormalMap";

        //Specular Map Texture Property
        [DisplayName("Specular Map Texture")]
        [Description("This will be used as the specular map on model, if not set will use a default")]
        [DefaultValue("")]
        public string SpecularMapTexture
        {
            get { return specularMapTexture; }
            set { specularMapTexture = value; }
        }
        private string specularMapTexture;

        //Specular Map Key, will be used to search for the specular map in the opaque data of the model
        [DisplayName("Specular Map Key")]
        [Description("This will be the key that will be used to search for the specular map in the opaque data of the model")]
        [DefaultValue("SpecularMap")]
        public string SpecularMapKey
        {
            get { return specularMapKey; }
            set { specularMapKey = value; }
        }
        private string specularMapKey = "SpecularMap";

        //Turn the GenerateTangentFrames option to be always on
        [Browsable(false)]
        public override bool GenerateTangentFrames
        {
            get { return true; }
            set { }
        }

        //Acceptable Vertex Channel Names Array
        static IList<string> acceptableVertexChannelNames = new string[]
        {
            VertexChannelNames.TextureCoordinate(0),
            VertexChannelNames.Normal(0),
            VertexChannelNames.Binormal(0),
            VertexChannelNames.Tangent(0),
        };

        //Process Vertex Channel
        protected override void ProcessVertexChannel(GeometryContent geometry, int vertexChannelIndex, ContentProcessorContext context)
        {
            //Get the Vertex Channel Name to be processed
            string vertexChannelName = geometry.Vertices.Channels[vertexChannelIndex].Name;

            //If this vertex channel has an acceptable name, process it as normal, else remove it
            if (acceptableVertexChannelNames.Contains(vertexChannelName))
            {
                base.ProcessVertexChannel(geometry, vertexChannelIndex, context);
            }
            else
            {
                geometry.Vertices.Channels.Remove(vertexChannelName);
            }
        }

        //Process Function
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            //Check if input is not NULL
            if (input == null) throw new ArgumentNullException("input");

            //Get the Directory
            directory = Path.GetDirectoryName(input.Identity.SourceFilename);

            //Get the texture filenames
            LookUpTextures(input);

            //Let the processing continue...
            return base.Process(input, context);
        }

        //Lookup the Textures for a given Node
        private void LookUpTextures(NodeContent node)
        {
            //Use node as MeshContent
            MeshContent mesh = node as MeshContent;

            //If mesh exists...
            if (mesh != null)
            {
                #region Normal Map Path Lookup
                //This will contain the path to the normal map texture...
                string normalMapPath;

                //Check if NormalMapTexture was set by the user, otherwise check if it's specified in the Model
                if (!string.IsNullOrEmpty(NormalMapTexture))
                {
                    normalMapPath = NormalMapTexture;
                }
                else
                {
                    //Check the model for the normal map
                    normalMapPath = mesh.OpaqueData.GetValue<string>(NormalMapKey, null);
                }

                //If still nothing check if a NormalMap with the meshes name and a _n.dds exists, if not then set the default
                if (normalMapPath == null)
                {
                    //Set path as a key of the mesh + _n.dds
                    normalMapPath = Path.Combine(directory, mesh.Name + "_n.dds");

                    //If a File with that name doesn't exist, just set to Default
                    if (!File.Exists(normalMapPath)) normalMapPath = "DefaultNormalMap.dds";
                }
                else
                {
                    normalMapPath = Path.Combine(directory, normalMapPath);
                }
                #endregion

                #region Specular Map Path Lookup
                //This will contain the path to the specular map texture...
                string specularMapPath;

                //Check if SpecularMapTexture was set by the user, otherwise check if it's specified in the Model
                if (!string.IsNullOrEmpty(SpecularMapTexture))
                {
                    specularMapPath = SpecularMapTexture;
                }
                else
                {
                    //Check the model for the specular map
                    specularMapPath = mesh.OpaqueData.GetValue<string>(SpecularMapKey, null);
                }

                //If still nothing check if a SpecularMap with the meshes name and a _s.dds exists, if not then set the default
                if (specularMapPath == null)
                {
                    //Set path as a key of the mesh + _n.dds
                    specularMapPath = Path.Combine(directory, mesh.Name + "_s.dds");

                    //If a File with that name doesn't exist, just set to Default
                    if (!File.Exists(specularMapPath)) specularMapPath = "DefaultSpecularMap.dds";
                }
                else
                {
                    specularMapPath = Path.Combine(directory, specularMapPath);
                }
                #endregion

                //Add the keys to the material, so they can be used by the shader
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    //In some .fbx files, the key might be found in the textures collection, but not
                    //in the mesh, as we checked above. If this is the case, we need to get it out, and
                    //add it with the "NormalMap" key
                    if (geometry.Material.Textures.ContainsKey(normalMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[normalMapKey];
                        geometry.Material.Textures.Remove(normalMapKey);
                        geometry.Material.Textures.Add("NormalMap", texRef);
                    }
                    else
                    {
                        geometry.Material.Textures.Add("NormalMap", new ExternalReference<TextureContent>(normalMapPath));
                    }

                    if (geometry.Material.Textures.ContainsKey(specularMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[specularMapKey];
                        geometry.Material.Textures.Remove(specularMapKey);
                        geometry.Material.Textures.Add("SpecularMap", texRef);
                    }
                    else
                    {
                        geometry.Material.Textures.Add("SpecularMap", new ExternalReference<TextureContent>(specularMapPath));
                    }
                }
            }

            //Check each child under the node as well
            foreach (NodeContent child in node.Children)
            {
                LookUpTextures(child);
            }
        }

        //Convert the Material Content
        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            //Initialize Effect Material Content
            EffectMaterialContent deferredShadingMaterial = new EffectMaterialContent();

            //Set Effect to the Effect Material Content
            deferredShadingMaterial.Effect = new ExternalReference<EffectContent>("Effects/GBuffer.fx");

            //Set the Textures to the Effect
            foreach (KeyValuePair<string, ExternalReference<TextureContent>> texture in material.Textures)
            {
                if ((texture.Key == "Texture") || (texture.Key == "NormalMap") || (texture.Key == "SpecularMap"))
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);
            }

            //Return Converted Material
            return context.Convert<MaterialContent, MaterialContent>(deferredShadingMaterial, typeof(MaterialProcessor).Name);
        }
    }
}
