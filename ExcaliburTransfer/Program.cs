// This code was used for a quick tool to transfer the Excalibur dungeon from Star Story to The Fairy Tale
// And making sure all texture files were properly copied but ONLY if needed! 
// You may view and download this file for evaluation purposes only!
//
// (c) Jeroen P. Broks

using NSKthura;
using System;
using System.Collections.Generic;
using System.IO;
using TrickyUnits;
using UseJCR6;

namespace ExcaliburTransfer {
	class ET {
		#region global
		static string TFTJCRFile => Dirry.AD("Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/Debug/The Fairy Tale REVAMPED.jcr");
		static string SSGJCRFile => Dirry.AD("Scyndi:Releases/Star Story Remake/Debug/Star Story.jcr");
		static string ConfigFile => Dirry.AD("Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/dev/ExcaliburTransfer/Config.ini");
		static string TFTSource => Dirry.AD("Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/src");
		static GINIE Config;
		static TJCRDIR TFT; // The Fairy Tale
		static TJCRDIR SSG; // Star Story Game
		
		static List<ET> Lijst = new List<ET>();

		static string Ask(string cat,string key,string Question,string DefVal = "") {
			while (Config[cat, key] =="") {
				if (DefVal != "") QCol.Magenta($"[{DefVal}] ");
				QCol.Yellow(Question);
				QCol.Cyan(" ");
				var Answer = Console.ReadLine();
				if (Answer == "") Answer = DefVal;
				Config[cat, key] = Answer;
			}
			return Config[cat, key];
		}
		#endregion

		#region Data
		readonly string _original, _target;
		string Original => Dirry.AD(_original);
		string Target => Dirry.AD(_target);
		Kthura OriMap;
		Kthura TarMap;
		
		#endregion

		#region Init
		ET(string ori,string tar) {
			Lijst.Add(this);
			_original = ori;
			_target = tar;
			Console.WriteLine();
		}
		#endregion

		#region Run
		static string HE(TJCRDIR D,string entry) {
			if (!D.Exists(entry)) return "";
			var data = D.LoadString(entry);
			return $"{qstr.md5(data)}.{data.Length}";
		}

		static string FindEquivalent(string tex) {
			var orihash = HE(SSG, tex);
			QCol.Doing("Finding an equivalent for", tex);
			foreach (var h in TFT.Entries) {
				try {
					// SEAL gives problems, and is not used in Star Story anyway, so it could never be the file I'm looking for.
					if (qstr.ExtractExt(h.Key) == "PNG" && h.Key!= "GFX/TEXTURES/SEAL/SEAL.PNG") {
						var tarhash = HE(TFT, h.Key);
						if (orihash == tarhash) {
							QCol.Yellow("Original texture ");
							QCol.Cyan(tex);
							QCol.Yellow(" appears to match target texture ");
							QCol.Cyan(h.Key);
							QCol.Magenta($"\n{tarhash}\n");
							QCol.Yellow("Use this in target game? ");
							bool chosen = false;
							do {
								var k = Console.ReadKey(true);
								switch (k.Key) {
									case ConsoleKey.Y:
									case ConsoleKey.J:
										QCol.Green("Yes\n");
										return h.Key;
									case ConsoleKey.N:
										QCol.Red("No");
										chosen = false;
										break;
								}
							} while (!chosen);
						}
					}
				} catch(Exception e) {
					QCol.QuickError($"Analysing {h.Value.Entry} went wrong! => {e.Message}!");
				}
			}
			return "";
		}

		static void Run() { foreach (var E in Lijst) E.Bekijk(); }

		void DoLayer(string Name, KthuraLayer Lay) {
			int itemnr = 0;
			QCol.Doing("=>", Name, "\t"); QCol.Magenta($"({Lay.Objects.Count} objects)\n");
			TarMap.CreateLayer(Name);
			var TarLay = TarMap.Layers[Name];
			for (int i = 0; i < Lay.Objects.Count; ++i) {
				KthuraObject to=null;
				bool okay = true;
				QCol.Blue($"Object: {i + 1}/{Lay.Objects.Count}\r");
				var oo = Lay.Objects[i];
				if (oo.kind == "$Item") {
					to = new KthuraObject("Obstacle", TarLay);
					to.Texture = "GFX/Treasure/Bag.png";
					to.Tag = Fmt.sprintf("RNDITEM_%08X", ++itemnr);
					okay = true;
				} else if (oo.kind[0] == '$') {
					var A = Ask("DollarTeken", oo.kind, $"Transfer special kind '{oo.kind}' as: ");
					okay = A.ToUpper() != "NOTHING";
					if (okay) to = new KthuraObject(Config["DollarTeken", oo.kind], TarLay);
				} else {
					to = new KthuraObject(oo.kind, TarLay);
				}
				if (okay) {
					to.x = oo.x;
					to.y = oo.y;
					to.w = oo.w;
					to.h = oo.h;
					to.Alpha255 = oo.Alpha255;
					to.R = oo.R;
					to.G = oo.G;
					to.B = oo.B;
					to.insertx = oo.insertx;
					to.inserty = oo.inserty;
					to.Impassible = oo.Impassible;
					to.ForcePassible = oo.ForcePassible;
					to.AnimFrame = oo.AnimFrame;
					to.AnimSpeed = oo.AnimSpeed;
					to.Dominance = oo.Dominance;
					to.Labels = oo.Labels;
					foreach (var it in oo.MetaData) to.MetaData[it.Key] = it.Value;
					to.RotationDegrees = oo.RotationDegrees;
					to.ScaleX = oo.ScaleX;
					to.ScaleY = oo.ScaleY;
					if (oo.kind!="$Item") to.Tag = oo.Tag;
					to.Visible = oo.Visible;
					if (oo.Texture != "") {
						while (Config["TextureChange", oo.Texture] == "") {
							if (qstr.ExtractExt(oo.Texture.ToLower()) == "jpbf") {
								QCol.Red("UNKNOWN JPBF FOUND!\n");
								QCol.Magenta($"{oo.Texture}\n");
								//var AE = SSG.Entries[oo.Texture.ToUpper()];
								//var AT = $"{qstr.md5(AE.Notes)}_{AE.Author}";
								//var TA = Ask("AuthorCopy", AT, $"To which author should I transfer texture \"{oo.Texture}\"?", AE.Author);
								var TA = "Transfer_Excalibur_From_StarStory";
								var TD = $"{TFTSource}/{TA}";
								var JT = $"{oo.Texture.ToUpper().Replace("GFX/TEXTURES/", "GFX/Textures/Excalibur/") }";
								var TT = $"{TD}/{JT}";
								Config["TextureChange", oo.Texture] = JT;
								//if (!File.Exists(TT)) {
								if (true) { // Seems odd, but it saves me a lot of work (for now)
									QCol.Doing("Creating", TT);
									Directory.CreateDirectory(qstr.ExtractDir(TT));
									var J = new TJCRCreate(TT, "zlib");
									var U = oo.Texture.ToUpper();
#if blockwork
								var L = new List<TJCREntry>();
#endif
									foreach (var E in SSG.Entries) {
										if (qstr.ExtractDir(E.Key) == U) {
											var buf = SSG.JCR_B(E.Key);
											if (buf.Length == 0) {
												QCol.QuickError($"Buffer {U} appears to be empty!");
											}
#if blockwork
										L.Add(E.Value);
#else
											QCol.Doing("Frame", qstr.StripDir(E.Value.Entry));
											J.AddBytes(buf, qstr.StripDir(E.Value.Entry), "zlib", E.Value.Author, E.Value.Notes);
#endif
										}
									}
#if blockwork
								if (L.Count == 1) {
									var E = L[0];
									J.AddBytes(SSG.JCR_B(U), qstr.StripDir(E.Entry), E.Author, E.Notes);
								} else {
									foreach (var E in L) {
										var B = new TJCRCreateBlock(J, "zlib");
										B.AddBytes(SSG.JCR_B(U), qstr.StripDir(E.Entry), E.Author, E.Notes);
										QCol.Doing("Frame", qstr.StripDir(E.Entry));
									}
								}
#endif
									J.Close();
								}
								//throw new Exception("No jpbf conversion yet!");
							} else {
								string NT = "";
								try {
									NT = FindEquivalent(oo.Texture);
								} catch (Exception e) {
									QCol.QuickError(e.Message);
								}
								if (NT != "") {
									Config["TextureChange", oo.Texture] = NT;
								} else {
									var AE = SSG.Entries[oo.Texture.ToUpper()];
									var AT = $"{qstr.md5(AE.Notes)}_{AE.Author}";
									var TA = Ask("AuthorCopy", AT, $"To which author should I transfer texture \"{oo.Texture}\"?", AE.Author);
									var TD = $"{TFTSource}/{TA}";
									var JT = $"{oo.Texture.ToUpper().Replace("GFX/TEXTURES/", "GFX/Textures/Excalibur/") }";
									var TT = $"{TD}/{JT}";
									QCol.Doing("Extracting to", TT);
									Directory.CreateDirectory(qstr.ExtractDir(TT));
									QuickStream.SaveBytes(TT, SSG.JCR_B(oo.Texture));
									Config["TextureChange", oo.Texture] = JT;
									//throw new Exception($"Copying not yet set ({oo.Texture})");
								}
							}
						}
						to.Texture = Config["TextureChange", oo.Texture];
					}
				}
			}
		}

		void Bekijk() {
			QCol.Doing("Converting", _original);
			QCol.Doing(" To target", _target);
			OriMap = Kthura.Load(Original);
			TarMap = Kthura.Create();
			foreach (var L in OriMap.Layers) DoLayer(L.Key, L.Value);
			QCol.Doing("Saving", Target);
			KthuraSave.PSave(TarMap, Target, "", "Store", "Jeroen P. Broks", "May not be extracted from the game without prior written permission from Jeroen P. Broks");
		}
#endregion

#region int main(int a,char** args)
		static void Main(string[] args) {
			JCR6.ErrorCrash = true;
			Dirry.InitAltDrives();
			Config = GINIE.FromFile(ConfigFile);
			Config.AutoSaveSource = ConfigFile;
			JCR6_lzma.Init();
			JCR6_zlib.Init();
			new KthuraDrawFake();
			QCol.Doing("Reading", TFTJCRFile); TFT = JCR6.Dir(TFTJCRFile); if (TFT == null) { QCol.QuickError($"JCR6 Error: {JCR6.JERROR}"); TrickyDebug.AttachWait(); return; }
			QCol.Doing("Reading", SSGJCRFile); SSG = JCR6.Dir(SSGJCRFile); if (SSG == null) { QCol.QuickError($"JCR6 Error: {JCR6.JERROR}"); TrickyDebug.AttachWait(); return; }
			new ET("Scyndi:Projects/Applications/Apollo/Games/Star Story/src/Tricky Story/Kthura/Excalibur - Final", "Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/src/Tricky Private/Kthura/NGP_Dungeon_Excalibur1");
			new ET("Scyndi:Projects/Applications/Apollo/Games/Star Story/src/Tricky Story/Kthura/Excalibur - Final - 2", "Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/src/Tricky Private/Kthura/NGP_Dungeon_Excalibur2");
			new ET("Scyndi:Projects/Applications/Apollo/Games/Star Story/src/Tricky Story/Kthura/Excalibur - Final Boss", "Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/src/Tricky Private/Kthura/NGP_Dungeon_Excalibur3");
			Run();
			TrickyDebug.AttachWait();
		}
#endregion
	}
}
