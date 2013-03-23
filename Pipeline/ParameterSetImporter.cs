using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using TImport = GameLib.ParameterSet;

namespace Pipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentImporter(".parm", DisplayName = "Parameter Set Importer")]
    public class ParameterSetImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            return TImport.FromFile(filename);
            /*
            //System.Diagnostics.Debugger.Launch();
            TImport Parm = new TImport();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (!string.IsNullOrWhiteSpace(line) && line[0] != '#')
                    {
                        string key = "";
                        string value = "";
                        int i = 0;
                        while (i < line.Length && line[i] != '=')
                        {
                            //if (char.IsLetterOrDigit(line[i]))
                                key += line[i];

                            ++i;
                        }

                        ++i;

                        while (i < line.Length)
                        {
                            //if (char.IsLetterOrDigit(line[i]))
                                value += line[i];

                            ++i;
                        }

                        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                        {
                            System.Diagnostics.Debug.Assert(false, "ParameterSetImporter - Bad input: " + line);
                            continue;
                        }

                        Parm.AddParm(key, value);
                    }
                }
            }

            return Parm;*/
        }
    }
}
