﻿/*-------------------------------------------------------------------------
Copyright 2013 Microsoft Open Technologies, Inc.
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
--------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace GruntBuildProcess
{
    public sealed class ExecuteGruntTask : CodeActivity<string>
    {
        public InArgument<string> BuildPath { get; set; }
        const string GruntBAT = "ExecGrunt.bat";
        string GruntExec = Path.GetTempFileName();

        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Collect the sort command output. 
            if (!String.IsNullOrEmpty(e.Data))
            {
                GruntExec = GruntExec + e.Data + Environment.NewLine;
            }
        }

        protected override string Execute(CodeActivityContext context)
        {

            var GruntExe = BuildPath.Get(context) + "\\" + GruntBAT;
            string[] Resourcenames = this.GetType().Assembly.GetManifestResourceNames();
            GruntExec = "start...GruntExe:" + GruntExe;

            foreach (string rName in Resourcenames)
            {
                if (rName.EndsWith(GruntBAT))
                {
                    GruntExec = GruntExec + "_" + rName + "_" + Assembly.GetExecutingAssembly().GetManifestResourceStream(rName).Length.ToString();
                    Stream sRunGruntCli = Assembly.GetExecutingAssembly().GetManifestResourceStream(rName);//.CopyTo(File.OpenWrite(GruntExec));
                    using (Stream fRunGruntCli = File.Create(GruntExe))
                    {
                        sRunGruntCli.CopyTo(fRunGruntCli);
                        fRunGruntCli.Close();
                    }
                    return GruntExe;

                }
            }
            GruntExec = GruntExec + "...end";

            return GruntExec;

        }
    }
}