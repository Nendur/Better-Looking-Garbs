using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Ionic.Zip;
using System.Collections.Generic;

namespace BetterLookingGarbsInstall
{
    static class Installer
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                //User documents location
                docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                docs = Path.GetFullPath(Path.Combine(docs, "Paradox Interactive", "Crusader Kings II"));
                if (!File.Exists(Path.Combine(docs, "settings.txt")))
                    docs = DirectorySelectionDialog("The Crusader Kings II user documents path was not found. Select the correct path to continue the installation.", "settings.txt");

                //Directory Structure
                mods = Path.GetFullPath(Path.Combine(docs, "mod"));
                blgdir = Path.Combine(mods, "betterlookinggarbs");
                submods = Path.Combine(blgdir, "blgsubmods");
                string modzippath = Path.Combine(mods, "betterlookinggarbs.zip");
                string ck2 = null;

                //Check if the installer is up to date compared to the mod.
                /*if (workshop && Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName) != "blginstall.exe")
                {
                    FileInfo installfile = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
                    FileInfo zipfile = new FileInfo(modzip);
                    if (installfile.LastWriteTime < zipfile.LastWriteTime)
                    {
                        string blginstall = Path.Combine(mods, "blginstall.exe");
                        if (File.Exists(blginstall))
                            File.Delete(blginstall);
                        using (ZipFile zip = ZipFile.Read(modzip))
                        {
                            ZipEntry e = zip["betterlookinggarbsinstall.exe"];
                            e.FileName = "blginstall.exe";
                            e.Extract(mods, ExtractExistingFileAction.OverwriteSilently);
                        }
                        Process installer = Process.Start(blginstall, "-continue");
                        installer.WaitForExit(60000);
                        if (File.Exists(blginstall))
                            File.Delete(blginstall);
                        Environment.Exit(0);
                    }
                }*/

                //Main Window visual log
                log = new LogViewer();
                BLGWindow mainwindow = new BLGWindow(log);

                //Open log file
                log.Open(Path.Combine(mods, "betterlookinggarbs.log"));

                if (File.Exists("betterlookinggarbsinstall.sh"))
                    File.Delete("betterlookinggarbsinstall.sh");
                if (Directory.Exists("betterlookinggarbs")
                    && !Path.GetFullPath(Directory.GetCurrentDirectory()).Equals(mods, StringComparison.OrdinalIgnoreCase))
                {
                    log.Write("Installing mod files from " + Directory.GetCurrentDirectory() + " to " + mods);
                    MoveFile("betterlookinggarbs.mod", mods);
                    if (Directory.Exists("betterlookinggarbs") && Directory.Exists(blgdir))
                        Directory.Delete(blgdir, true);
                    MoveDir("betterlookinggarbs", mods);
                    MoveDir("blgsubmods", mods);
                    //MoveFile("betterlookinggarbsinstall.exe", blgtmp);
                }
                else if (File.Exists(modzippath))
                {
                    log.Write("Opening " + modzippath);

                    //Close other application holding the zip open.
                    string rmkey = Guid.NewGuid().ToString();
                    int res = RmStartSession(out uint rmhandle, 0, rmkey);
                    if (res != 0)
                        throw new Exception("Could not begin restart manager session.");

                    try
                    {
                        string[] resources = new string[] { modzippath };

                        res = RmRegisterResources(rmhandle, (uint)resources.Length, resources, 0, null, 0, null);
                        if (res != 0)
                            throw new Exception("Could not register restart manager file path.");
                        res = RmShutdown(rmhandle, 0, null);
                        if (res != 0)
                            throw new Exception("Could not close other applications holding betterlookinggarbs.zip open.");
                    }
                    finally
                    {
                        RmEndSession(rmhandle);
                    }

                    //Open mod archives.
                    modzip = ZipFile.Read(modzippath);
                    modzip.StatusMessageTextWriter = log.LogStream();
                    modzip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                    modzip.FlattenFoldersOnExtract = true;

                    /* e = modzip["betterlookinggarbs.fix"];
                    if (e == null)
                        log.Error(@"Could not open betterlookinggarbs.zip\betterlookinggarbs.fix");
                    else
                    {
                        e.Extract(mods, ExtractExistingFileAction.OverwriteSilently);
                        File.Delete(Path.Combine(mods, "betterlookinggarbs.mod"));
                        File.Delete(Path.Combine(mods, "betterlookinggarbs.mod.mod"));
                        File.Move(Path.Combine(mods, "betterlookinggarbs.fix"), Path.Combine(mods, "betterlookinggarbs.mod"));
                    }*/
                    ZipEntry e = modzip["betterlookinggarbsextra.zip"];
                    if (e == null)
                        log.Error(@"Could not open betterlookinggarbs.zip\betterlookinggarbsextra.zip");
                    extrazipmem = new MemoryStream((int)e.UncompressedSize);
                    e.Extract(extrazipmem);
                    extrazipmem.Seek(0, SeekOrigin.Begin);
                    extrazip = ZipFile.Read(extrazipmem);
                }

                //Verify installation directory
                if (extrazip == null)
                {
                    string extrazippath = Path.Combine(blgdir, "betterlookinggarbsextra.zip");
                    if (!File.Exists(extrazippath))
                    {
                        if (File.Exists("betterlookinggarbsextra.zip"))
                            extrazippath = Directory.GetCurrentDirectory();
                        else
                            extrazippath = DirectorySelectionDialog("The Better Looking Garbs extra files were not found. Select the correct path to betterlookinggarbsextra.zip.", "betterlookinggarbsextra.zip");
                        extrazippath = Path.Combine(extrazippath, "betterlookinggarbsextra.zip");
                    }
                    extrazip = ZipFile.Read(extrazippath);
                    //Directory.SetCurrentDirectory(submods);
                }
                //log.Write("Running installation from " + Directory.GetCurrentDirectory());

                //Install detected addons
                InstallAddons();

                //Search for the CKII install directory.
                bool duplicateinstalls = false;
                try
                {
                    string steam = (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Valve\\Steam", "SteamPath", null);
                    if (steam != null)
                    {
                        steam = Path.GetFullPath(steam);
                        log.Write("\nSteam located at " + steam);
                        steam = Path.Combine(steam, "SteamApps");
                    }
                    else
                    {
                        log.Write("\nSteam install location not found.");
                        steam = "C:\\Program Files (x86)\\steam\\SteamApps";
                    }

                    log.WriteDebug("Searching " + steam + "\\common\\Crusader Kings II");
                    if (Directory.Exists(Path.Combine(steam, "common", "Crusader Kings II")))
                        ck2 = Path.Combine(steam, "common", "Crusader Kings II");
                    if (File.Exists(Path.Combine(steam, "libraryfolders.vdf")))
					{
						using (StreamReader libraryfolders = new StreamReader(Path.Combine(steam, "libraryfolders.vdf")))
						{
							String line;
							libraryfolders.ReadLine();
							libraryfolders.ReadLine();
							libraryfolders.ReadLine();
							libraryfolders.ReadLine();
							while ((line = libraryfolders.ReadLine()) != null && line != "}")
							{
								String path = line.Split(new Char[] { '\"' }, StringSplitOptions.RemoveEmptyEntries)[3];
								path = Path.Combine(Path.GetFullPath(path), "SteamApps", "common", "Crusader Kings II");
								log.WriteDebug("Searching " + path);
								if (Directory.Exists(path))
								{
									if (ck2 == null)
										ck2 = path;
									else
										duplicateinstalls = true;
								}
							}
						}
					}
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                }

                if (ck2 != null && !duplicateinstalls)
                    log.Write("Crusader Kings II installation found in " + ck2 + '\n');
                else
                {
                    log.Write("Crusader Kings II installation location not found.\n");
                    ck2 = DirectorySelectionDialog((duplicateinstalls ? "There are multiple Crusader Kings II installations." : "The Crusader Kings II installation path was not found.")
                        + " Select the correct path to continue the installation.", "CK2Game.exe");
                    log.WriteDebug("Crusader Kings II input as " + ck2);
                }

                //The big extra files extraction list
                String dlc = Path.Combine(ck2, "DLC", "dlc");
                //if (Directory.Exists(blgdir))
                //    Directory.SetCurrentDirectory(blgdir);
                extrazip.StatusMessageTextWriter = log.LogStream();
                extrazip.Password = "hr37yw3tre4gg84y";
                extrazip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                extrazip.FlattenFoldersOnExtract = true;

                bool turkishunits = File.Exists(dlc + "040.dlc");
                bool persian = File.Exists(dlc + "044.dlc");
                bool southernunits = File.Exists(dlc + "051.dlc");
                bool southern = File.Exists(dlc + "052.dlc");

                //First Pass
                bool byzantine = File.Exists(dlc + "014.dlc");
                bool earlyeast = File.Exists(dlc + "047.dlc");
                if (byzantine || earlyeast)
                    ExtractDirectory(extrazip, "1dlc014odlc047");
                bool russian = File.Exists(dlc + "016.dlc");
                if (russian)
                    ExtractDirectory(extrazip, "1dlc016");
                bool norse = File.Exists(dlc + "020.dlc");
                if (norse)
                    ExtractDirectory(extrazip, "1dlc020");
                bool conclave = File.Exists(dlc + "063.dlc");
                if (conclave)
                    ExtractDirectory(extrazip, "1dlc063");
                bool holyfury = File.Exists(dlc + "074.dlc");
                if (holyfury)
                    ExtractDirectory(extrazip, "1dlc074");

                //Second Pass
                if (earlyeast)
                    ExtractDirectory(extrazip, "2dlc047");
                bool reapers = File.Exists(dlc + "067.dlc");
                if (reapers)
                    ExtractDirectory(extrazip, "2dlc067");
                bool monks = File.Exists(dlc + "070.dlc");
                if (monks)
                    ExtractDirectory(extrazip, "2dlc070");

                //Third Pass
                bool charlemagne = File.Exists(dlc + "045.dlc");
                if (charlemagne)
                    ExtractDirectory(extrazip, "3dlc045");

                //Main Pass
                bool mongol = File.Exists(dlc + "002.dlc");
                log.WriteDLCCheck("Mongol Face Pack (portraits, units)", mongol);
                if (mongol)
                {
                    ExtractDirectory(extrazip, "dlc002");
                    if (russian)
                        ExtractDirectory(extrazip, "dlc002adlc016");
                }

                bool africanunits = File.Exists(dlc + "008.dlc");
                //log.WriteDLCCheck("African Unit Pack (units)", africanunits);
                if (africanunits)
                    ExtractDirectory(extrazip, "dlc008");

                bool lor = File.Exists(dlc + "011.dlc");
                log.WriteDLCCheck("Legacy of Rome (councillors)", lor);
                if (lor)
                    ExtractDirectory(extrazip, "dlc011");

                bool byzantineunits = File.Exists(dlc + "012.dlc");
                if (byzantineunits)
                    ExtractDirectory(extrazip, "dlc012");

                bool african = File.Exists(dlc + "013.dlc");
                log.WriteDLCCheck("African Portraits (portraits)", african);
                if (african)
                    ExtractDirectory(extrazip, "dlc013");

                log.WriteDLCCheck("Mediterranean Portraits (portraits)", byzantine);
                if (byzantine || earlyeast)
                    ExtractDirectory(extrazip, "dlc014odlc047");
                if (byzantine)
                {
                    ExtractDirectory(extrazip, "dlc014");
                    if (russian)
                        ExtractDirectory(extrazip, "dlc014adlc016");
                }

                bool russianunits = File.Exists(dlc + "015.dlc");
                log.WriteDLCCheck("Russian Unit Pack (units)", russianunits);
                if (russianunits)
                {
                    ExtractDirectory(extrazip, "dlc015");
                    if (charlemagne)
                        ExtractDirectory(extrazip, "dlc015adlc045");
                }
                log.WriteDLCCheck("Russian Portraits (portraits)", russian);
                if (russian)
                    ExtractDirectory(extrazip, "dlc016");

                bool mesoamerican = File.Exists(dlc + "018.dlc");
                log.WriteDLCCheck("Sunset Invasion (portraits, units)", mesoamerican);
                if (mesoamerican)
                    ExtractDirectory(extrazip, "dlc018");

                log.WriteDLCCheck("Norse Portraits (portraits)", norse);
				if(norse)
					ExtractDirectory(extrazip, "dlc020");
                if (File.Exists(dlc + "021.dlc"))
                    ExtractDirectory(extrazip, "dlc021");

                bool republic = File.Exists(dlc + "022.dlc");
                if (republic)
                    ExtractDirectory(extrazip, "dlc022");

                bool oldgods = File.Exists(dlc + "024.dlc");
                log.WriteDLCCheck("The Old Gods (councillors)", oldgods);
                if (oldgods)
                    ExtractDirectory(extrazip, "dlc024");

                bool celtic = File.Exists(dlc + "028.dlc");
                log.WriteDLCCheck("Celtic Portraits (portraits)", celtic);
                if (celtic)
                    ExtractDirectory(extrazip, "dlc028");
                if (File.Exists(dlc + "027.dlc"))
                    ExtractDirectory(extrazip, "dlc027");

                bool holy = File.Exists(dlc + "033.dlc");
                bool wof = File.Exists(dlc + "034.dlc");
                log.WriteDLCCheck("Warriors of Faith Unit Pack (units, councillors)", wof);
                if (holy)
                    ExtractDirectory(extrazip, "dlc033");
                if (wof)
                {
                    ExtractDirectory(extrazip, "dlc034");
                    if (turkishunits)
                        ExtractDirectory(extrazip, "dlc034adlc040");
                }

                bool saxonunits = File.Exists(dlc + "037.dlc");
                if (saxonunits)
                    ExtractDirectory(extrazip, "dlc037");

                bool ugricunits = File.Exists(dlc + "038.dlc");
                log.WriteDLCCheck("Finno-Ugric Unit Pack (units)", ugricunits);
                if (ugricunits)
                {
                    ExtractDirectory(extrazip, "dlc038");
                    if (russianunits)
                        ExtractDirectory(extrazip, "dlc038adlc015");
                }

                bool indian = File.Exists(dlc + "039.dlc");
                log.WriteDLCCheck("Rajas of India (portraits, units, councillors)", indian);
                if (indian)
                {
                    ExtractDirectory(extrazip, "dlc039");
                    if (persian)
                        ExtractDirectory(extrazip, "dlc039adlc044");
                }

                log.WriteDLCCheck("Turkish Unit Pack (units)", turkishunits);
                if (turkishunits)
                    ExtractDirectory(extrazip, "dlc040");
                bool turkish = File.Exists(dlc + "041.dlc");
                log.WriteDLCCheck("Turkish Portraits (portraits)", turkish);
                if (turkish)
                    ExtractDirectory(extrazip, "dlc041");

                log.WriteDLCCheck("Persian Portraits (portraits)", persian);
                if (persian)
                    ExtractDirectory(extrazip, "dlc044");
                if (File.Exists(dlc + "043.dlc") && indian)
                    ExtractDirectory(extrazip, "dlc043adlc039");

                log.WriteDLCCheck("Charlemagne (units)", charlemagne);
                if (charlemagne)
                {
                    if (!holy)
                        ExtractDirectory(extrazip, "dlc033rdlc045");
                    if (turkishunits)
                        ExtractDirectory(extrazip, "dlc045adlc040");
                    if (southernunits)
                        ExtractDirectory(extrazip, "dlc045adlc051");
                    if (File.Exists(dlc + "058.dlc"))
                        ExtractDirectory(extrazip, "dlc045adlc058");
                }

                bool earlywest = File.Exists(dlc + "046.dlc");
                log.WriteDLCCheck("Early Western Clothing Pack (clothing)", earlywest);
                if (earlywest)
                {
                    ExtractDirectory(extrazip, "dlc046");
                    if (byzantine)
                        ExtractDirectory(extrazip, "dlc046adlc014");
                    if (southern)
                        ExtractDirectory(extrazip, "dlc046adlc052");
                }

                log.WriteDLCCheck("Early Eastern Clothing Pack (clothing)", earlyeast);
                if (earlyeast)
                    ExtractDirectory(extrazip, "dlc047");

                log.WriteDLCCheck("Iberian Portraits (portraits)", southern);
                if (southern)
                    ExtractDirectory(extrazip, "dlc052");

                bool horselords = File.Exists(dlc + "057.dlc");
                log.WriteDLCCheck("Horse Lords Content Pack (portraits, units)", horselords);
                if (horselords)
                {
                    ExtractDirectory(extrazip, "dlc057");
                    if (!turkish)
                        ExtractDirectory(extrazip, "dlc041rdlc057");
                }

                if (File.Exists(dlc + "058.dlc"))
                {
                    if (!holy)
                        ExtractDirectory(extrazip, "dlc033rdlc058");
                    if (charlemagne)
                        ExtractDirectory(extrazip, "dlc045adlc058");
                }

                if (File.Exists(dlc + "060.dlc"))
                    ExtractDirectory(extrazip, "dlc060");

                log.WriteDLCCheck("Conclave Content Pack (portraits, councillors)", conclave);
                if (conclave)
                {
                    ExtractDirectory(extrazip, "dlc063");
                    if (republic)
                        ExtractDirectory(extrazip, "dlc022adlc063");
                }
                else if (celtic && mongol && norse)
                    ExtractDirectory(extrazip, "dlc063rdlc002adlc020adlc028");

                bool southindian = File.Exists(dlc + "065.dlc") || File.Exists(dlc + "072.dlc");
                log.WriteDLCCheck("South Indian Portraits (portraits)", southindian);
                if (southindian)
                {
                    ExtractDirectory(extrazip, "dlc065odlc072");
                    if (indian)
                        ExtractDirectory(extrazip, "dlc039adlc065odlc072");
                    else
                        ExtractDirectory(extrazip, "dlc039rdlc065odlc072");
                }

                log.WriteDLCCheck("The Reaper's Due Content Pack (portraits, councillors)", reapers);
                if (reapers)
                    ExtractDirectory(extrazip, "dlc067");
                else if (african)
                    ExtractDirectory(extrazip, "dlc067rdlc013");

                log.WriteDLCCheck("Monks and Mystics Content Pack (portraits, units)", monks);
                if (monks)
                {
                    ExtractDirectory(extrazip, "dlc070");
                }
                else
                {
                    if (saxonunits)
                        ExtractDirectory(extrazip, "dlc070rdlc037");
                    if (charlemagne)
                        ExtractDirectory(extrazip, "dlc070rdlc045");
                    if (File.Exists(dlc + "059.dlc"))
                        ExtractDirectory(extrazip, "dlc070rdlc059");
                }

                bool jadedragon = File.Exists(dlc + "073.dlc");
                log.WriteDLCCheck("Jade Dragon (portraits, units)", jadedragon);
                if (jadedragon)
                {
                    ExtractDirectory(extrazip, "dlc073");
                    if (horselords)
                        ExtractDirectory(extrazip, "dlc073adlc057");
                }

                log.WriteDLCCheck("Holy Fury (portraits, councillors, units)", holyfury);
                if (holyfury)
                {
                    ExtractDirectory(extrazip, "dlc074");
                    if (russian)
                        ExtractDirectory(extrazip, "dlc074adlc016");
                    if (earlywest)
                        ExtractDirectory(extrazip, "dlc074adlc046");
                 }
                else
                {
                    if (monks)
                        ExtractDirectory(extrazip, "dlc074rdlc070");
                }

                MergeMod(extrazip);
                extrazip.Dispose();
                log.Write("\nFinished extractions.");

                //Merge the extra files with the base mod
                //bool mergeddlc = false;
                if (modzip != null)
                {
                    //Update mod archive.
                    /*log.Write("Merging extra files with betterlookinggarbs.zip.");
                    string[] subdirs = Directory.GetDirectories(blgtmp, "dlc*");
                    foreach (string dir in subdirs)
                    {
                        modzip.UpdateDirectory(dir);
                        mergeddlc = true;
                    }*/

                    ZipEntry installfile = modzip["common\\on_actions\\BLG_installcheck.txt"];
                    if (installfile != null)
                        modzip.RemoveEntry(installfile);
                    string tmpzipfile = modzippath + ".tmp";
                    modzip.Save(tmpzipfile);
                    modzip.Dispose();
                    try
                    {
                        File.Delete(modzippath);
                        File.Move(tmpzipfile, modzippath);
                    }
                    catch (Exception e)
                    {
                        log.Error(e.Message);
                        string message = "The file " + modzippath + " could not be written to. Replace betterlookinggarbs.zip with betterlookinggarbs.zip.tmp to complete the installation.";
                        log.Error(message);
                        MessageBox.Show(message);
                    }
                }
                if (Directory.Exists(blgdir))
                {
                    /*log.Write("Merging extra files with betterlookinggarbs.");
                    string[] subdirs = Directory.GetDirectories(blgtmp, "dlc*");
                    foreach (string dir in subdirs)
                    {
                        Directory.SetCurrentDirectory(dir);
                        if (Directory.Exists("gfx"))
                            MoveDir("gfx", blgdir);
                        if (Directory.Exists("interface"))
                            MoveDir("interface", blgdir);
                        Directory.SetCurrentDirectory("..");
                        Directory.Delete(dir);
                        mergeddlc = true;
                    }*/
                    string installcheck = Path.Combine(blgdir, "common", "on_actions", "BLG_installcheck.txt");
                    if (File.Exists(installcheck))
                        File.Delete(installcheck);
                }

                /*blgtmp = Path.Combine(mods, "betterlookinggarbsaddons");
                if (modzip != null && Directory.Exists(blgtmp))
                {
                    Directory.SetCurrentDirectory(mods);
                    Directory.Delete(blgtmp, true);
                }

                if (!mergeddlc)
                    log.Write("Better Looking Garbs installation completed without installing DLC dependent files. If this is incorrect check the betterlookinggarbs.log file for problems.");
                else*/
                log.Write("Better Looking Garbs installation complete.");
                MessageBox.Show("Better Looking Garbs installation complete.");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                MessageBox.Show(e.Message);
            }
        }

        static String docs;
        static String mods;
        static String blgdir;
        static String submods;
        static ZipFile modzip = null;
        static ZipFile extrazip = null;
        static MemoryStream extrazipmem;

        static Dictionary<string, ZipEntry> filestoextract = new Dictionary<string, ZipEntry>();

        static LogViewer log;

        // Move and overwrite file.
        static void MoveFile(String source, String destdir)
        {
            try
            {
                String dest = Path.Combine(destdir, source);
                if (File.Exists(source))
                {
                    if (File.Exists(dest))
                        File.Delete(dest);
                    File.Move(source, dest);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        //Move and overwrite files and directories.
        static void MoveDir(String source, String dest)
        {
            try
            {
                if (Directory.Exists(source))
                {
                    Directory.CreateDirectory(Path.Combine(dest, source));
                    string[] files = Directory.GetFiles(source);
                    foreach (string file in files)
                        MoveFile(file, dest);

                    // Recurse into subdirectories of this directory. 
                    string[] subdirs = Directory.GetDirectories(source);
                    foreach (string dir in subdirs)
                        MoveDir(dir, dest);

                    Directory.Delete(source);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        // Select a directory with all underlying files for inclusion.
        static void ExtractDirectory(ZipFile zip, String dir)
        {
            bool direxists = false;
            foreach (ZipEntry file in zip)
                if (!file.IsDirectory && file.FileName.StartsWith(dir + '/'))
                {
                    string destination = file.FileName.Substring(dir.Length + 1);   //Cut dlc directory
                    filestoextract[destination] = file;
                    direxists = true;
                    /*if (modzip != null)
                    {
                        MemoryStream buffer = new MemoryStream((int)file.UncompressedSize);
                        file.Extract(buffer);
                        buffer.Seek(0, SeekOrigin.Begin);
                        modzip.UpdateEntry(destination, buffer);
                    }
                    if (Directory.Exists(blgdir))
                        file.Extract(Path.GetDirectoryName(destination), ExtractExistingFileAction.OverwriteSilently);*/
                }
            if (!direxists)
                log.Error("The directory " + dir + " does not exists in the installation archive.");
        }


        // Extract and merge a files to the main mod.
        static void MergeMod(ZipFile zip)
        {
            foreach (KeyValuePair<string, ZipEntry> filepair in filestoextract)
            {
                log.WriteDebug("Extracting " + filepair.Value.FileName);
                if (modzip != null)
                {
                    MemoryStream buffer = new MemoryStream((int)filepair.Value.UncompressedSize);
                    filepair.Value.Extract(buffer);
                    buffer.Seek(0, SeekOrigin.Begin);
                    modzip.UpdateEntry(filepair.Key, buffer);
                }
                if (Directory.Exists(blgdir))
                    filepair.Value.Extract(Path.Combine(blgdir, Path.GetDirectoryName(filepair.Key)), ExtractExistingFileAction.OverwriteSilently);
            }
        }

        static string DirectorySelectionDialog(string description, string testfile)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                Description = description,
                ShowNewFolderButton = false
            };
            while (true)
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    Environment.Exit(1);
                if (File.Exists(Path.Combine(dialog.SelectedPath, testfile)))
                    return dialog.SelectedPath;
            }
        }

        static void InstallAddons()
        {
            try
            {
                if ((File.Exists(Path.Combine(mods, "Historical_Immersion_Project\\SWMH_changelog.txt")) ||
					File.Exists(Path.Combine(mods, "Historical Immersion Project\\SWMH_changelog.txt")) ||
                    File.Exists(Path.Combine(mods, "blgccswmh.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgcchip.zip")))
                {
                    log.Write("HIP SWMH compatibility add-on installed.");
                    InstallAnAddon("blgccswmh");
                }
                if ((File.Exists(Path.Combine(mods, "Historical_Immersion_Project\\EMF_changelog.txt")) ||
					File.Exists(Path.Combine(mods, "Historical Immersion Project\\EMF_changelog.txt")) ||
                    File.Exists(Path.Combine(mods, "blgccemf.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgcchip.zip")))
                {
                    log.Write("HIP EMF compatibility add-on installed.");
                    InstallAnAddon("blgccemf");
                }
                if (File.Exists(Path.Combine(mods, "Historical_Immersion_Project\\CPRplus_attributions.txt")) ||
					File.Exists(Path.Combine(mods, "Historical Immersion Project\\CPRplus_attributions.txt")))
                    log.Error("Note: Do not use Better Looking Garbs in combination with CPRplus.");
                if ((Directory.Exists(Path.Combine(mods, "CK2Plus")) ||
                    File.Exists(Path.Combine(mods, "ck2plus.zip")) ||
                    File.Exists(Path.Combine(mods, "blgccck2+.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgccck2+.zip")))
                {
                    log.Write("CK2 Plus compatibility add-on installed.");
                    InstallAnAddon("blgccck2+");
                }
                if ((Directory.Exists(Path.Combine(mods, "A Game of Thrones")) ||
                    File.Exists(Path.Combine(mods, "a game of thrones.zip")) ||
                    File.Exists(Path.Combine(mods, "blgccagot.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgccagot.zip")))
                {
                    log.Write("A Game of Thrones compatibility add-on installed.");
                    InstallAnAddon("blgccagot");
                    if (Directory.Exists("blgagot"))
                        Directory.Delete("blgagot", true);
                    if (File.Exists("blgagot.mod"))
                        File.Delete("blgagot.mod");
                }
                if ((Directory.Exists(Path.Combine(mods, "EK021")) ||
                    Directory.Exists(Path.Combine(mods, "EKSVN")) ||
                    File.Exists(Path.Combine(mods, "EK021.mod")) ||
                    File.Exists(Path.Combine(mods, "ESM.mod")) ||
                    File.Exists(Path.Combine(mods, "blgccelderkings.mod"))) &&
                        !File.Exists(Path.Combine(mods, "blgccelderkings.zip")))
                {
                    log.Write("Elder Kings compatibility add-on installed.");
                    InstallAnAddon("blgccelderkings");
                }
                if ((Directory.Exists(Path.Combine(mods, "WTWSMS")) ||
                    File.Exists(Path.Combine(mods, "wtwsms.zip")) ||
                    File.Exists(Path.Combine(mods, "blgccwtwsms.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgccwtwsms.zip")))
                {
                    log.Write("WTWSMS compatibility add-on installed.");
                    InstallAnAddon("blgccwtwsms");
                }
                if ((Directory.Exists(Path.Combine(mods, "Britannia")) ||
                    File.Exists(Path.Combine(mods, "britannia.zip")) ||
                    File.Exists(Path.Combine(mods, "blgccwinterking.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgccwinterking.zip")))
                {
                    log.Write("The Winter King compatibility add-on installed.");
                    InstallAnAddon("blgccwinterking");
                }
                if ((Directory.Exists(Path.Combine(mods, "Lux Invicta")) ||
                    File.Exists(Path.Combine(mods, "Lux Invicta.mod")) ||
                    File.Exists(Path.Combine(mods, "blgccluxinvicta.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgccluxinvicta.zip")))
                {
                    log.Write("Lux Invicta compatibility add-on installed.");
                    InstallAnAddon("blgccluxinvicta");
                }
                if ((Directory.Exists(Path.Combine(mods, "Tianxia")) ||
                    File.Exists(Path.Combine(mods, "Tianxia.mod")) ||
                    File.Exists(Path.Combine(mods, "blgcctianxia.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgcctianxia.zip")))
                {
                    log.Write("Tianxia compatibility add-on installed.");
                    InstallAnAddon("blgcctianxia");
                }
                if ((File.Exists(Path.Combine(mods, "mythos3.mod")) ||
                    File.Exists(Path.Combine(mods, "mythos3.zip")) ||
                    File.Exists(Path.Combine(mods, "Mythos2.mod")) ||
                    File.Exists(Path.Combine(mods, "blgccmythos.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgccmythos.zip")))
                {
                    log.Write("Mythos compatibility add-on installed.");
                    InstallAnAddon("blgccmythos");
                }
                if ((Directory.Exists(Path.Combine(mods, "After the End Fan Fork")) ||
                    File.Exists(Path.Combine(mods, "aftertheendfanfork.zip")) ||
                    File.Exists(Path.Combine(mods, "blgccaftertheend.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgccaftertheend.zip")))
                {
                    log.Write("After the End compatibility add-on installed.");
                    InstallAnAddon("blgccaftertheend");
                }
                if ((Directory.Exists(Path.Combine(mods, "Rise to Power")) ||
                    File.Exists(Path.Combine(mods, "risetopower.zip")) ||
                    File.Exists(Path.Combine(mods, "blgrisetopower.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgrisetopower.zip")))
                {
                    log.Write("Rise to Power compatibility add-on installed.");
                    InstallAnAddon("blgrisetopower");
                }
                if ((Directory.Exists(Path.Combine(mods, "Dark World Reborn")) ||
                    File.Exists(Path.Combine(mods, "Dark World Reborn.mod")) ||
                    Directory.Exists(Path.Combine(mods, "LuxuriaFantasia")) ||
                    File.Exists(Path.Combine(mods, "LuxuriaFantasia.mod")) ||
                    File.Exists(Path.Combine(mods, "blgdarkworld.mod"))) &&
                    !File.Exists(Path.Combine(mods, "blgdarkworld.zip")))
                {
                    log.Write("Dark World compatibility add-on installed.");
                    InstallAnAddon("blgdarkworld");
                }
                if ((File.Exists(Path.Combine(mods, "blgccironman.mod")) ||
                    File.ReadAllText(Path.Combine(docs, "settings.txt")).Contains("ironman=yes")) &&
                    !File.Exists(Path.Combine(mods, "blgccironman.zip")))
                {
                    log.Write("Ironman add-on installed.");
                    InstallAnAddon("blgccironman");
                }
                if (File.Exists(Path.Combine(mods, "blgccgeneric.mod")) &&
                    !File.Exists(Path.Combine(mods, "blgccgeneric.zip")))
                {
                    log.Write("Generic compatibility add-on installed.");
                    InstallAnAddon("blgccgeneric");
                }
                if (File.Exists(Path.Combine(mods, "blgnodisease.mod")) &&
                    !File.Exists(Path.Combine(mods, "blgnodisease.zip")))
                {
                    log.Write("Vanilla disease graphics add-on installed.");
                    InstallAnAddon("blgnodisease");
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        static void InstallAnAddon(string name)
        {
            if (modzip == null && (!Directory.Exists(Path.Combine(submods, name))))
                return;
            string dest = Path.Combine(mods, name);
            if (Directory.Exists(dest))
                Directory.Delete(dest, true);
            if (modzip != null)
            {
                foreach (ZipEntry file in modzip)
                    if (!file.IsDirectory && file.FileName.StartsWith("submods/" + name))
                    {
                        string destination = mods + Path.GetDirectoryName(file.FileName).Substring(7);
                        file.Extract(destination, ExtractExistingFileAction.OverwriteSilently);
                    }
            }
            else
            {
                name = Path.Combine(submods, name);
                MoveFile(name + ".mod", mods);
                MoveDir(name, mods);
            }
        }

        //Restart Manager imports:
        [StructLayout(LayoutKind.Sequential)]
        struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
        }

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        static extern int RmRegisterResources(uint pSessionHandle, UInt32 nFiles, string[] rgsFilenames, UInt32 nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, UInt32 nServices, string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
        static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        static extern int RmEndSession(uint pSessionHandle);

        delegate void RM_WRITE_STATUS_CALLBACK(UInt32 nPercentComplete);
        [DllImport("rstrtmgr.dll")]
        static extern int RmShutdown(uint pSessionHandle, ulong lActionFlags, RM_WRITE_STATUS_CALLBACK fnStatus);
    }
}