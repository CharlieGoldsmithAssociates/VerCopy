/* *****************************************************************************
 *  VerCopy
 * A trivial utility used in the build of HRIS etc, the code-singing batch files use this tool to create
 * a copy of the exe named using it's internal version number, and also a zip file of the same for ease of emailing.
 *
 * usage VerCopy <path to exe>
 * 
 *  Initial version HowardT, May 2014.
 *  Charlie Goldsmith Associates Ltd
 *
 * Copyright (C) 2015 Charlie Goldsmith Associates Ltd
 * All rights reserved.
 *
 * Charlie Goldsmith Associates Ltd develop and use this software as part of its work
 * but the software itself is open-source software; you can redistribute it and/or modify
 * it under the terms of the BSD licence below
 *
 *Redistribution and use in source and binary forms, with or without modification,
 *are permitted provided that the following conditions are met:
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 * 3. Neither the name of the copyright holder nor the names of its contributors
 *    may be used to endorse or promote products derived from this software 
 *    without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
 * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY 
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 *
 *for more information please see http://opensource.org/licenses/BSD-3-Clause
 * *****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO.Compression;

namespace VerCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 )
            {
                System.Console.WriteLine("Usage vercopy <path to exe> \n");
                System.Console.WriteLine("uses file version to create a .exe.NNNNNN copy and a zip\n");

            }
            foreach( string sArg in args )
            {
                FileInfo fi = new FileInfo(sArg);
                if ( !fi.Exists)
                {
                    System.Console.WriteLine("Cannot find source exe " + sArg + "\n");
                }
                else
                {
                    
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(sArg);
                    System.Console.WriteLine("Processing " + sArg  + ":" +  versionInfo.FileMajorPart.ToString() +
                         "." + versionInfo.FileMinorPart.ToString() +
                         "." + versionInfo.FileBuildPart.ToString() + 
                         "\n");

                    // eg 1.7.3 or 1.17.33 becomes 10703 or 11733
                    int iVersion = (versionInfo.FileMajorPart * 100000) + (versionInfo.FileMinorPart * 1000) + versionInfo.FileBuildPart;
                    string sDest = sArg + "." + iVersion.ToString();
                    fi.CopyTo(sDest,true);

                    // and add a zip file too,.
                    string sZip = sDest + ".zip";
                    if (File.Exists(sZip)) File.Delete(sZip);

                    ZipStorer zS = ZipStorer.Create(sZip,"Zip for release " + versionInfo.FileMajorPart.ToString() +
                         "." + versionInfo.FileMinorPart.ToString() +
                         "." + versionInfo.FileBuildPart.ToString() + " of " + sArg);
                    zS.AddFile( ZipStorer.Compression.Deflate, sDest, fi.Name+ "." + iVersion.ToString() , "");
                    zS.Close();
                    
                }

            }
        }
    }
}
