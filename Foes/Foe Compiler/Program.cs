// Lic:
// **********************************************
// 
// This file is part of a closed-source
// project by Jeroen Petrus Broks and should
// therefore not be in your pocession without
// his permission which should be obtained
// PRIOR to obtaining this file.
// 
// You may not distribute this file under
// any circumstances or distribute the
// binary file it procudes by the use of
// compiler software without PRIOR written
// permission from Jeroen P. Broks.
// 
// If you did obtain this file in any way
// please remove it from your system and
// notify Jeroen Broks you got it somehow. If
// you have downloaded it from a website
// please notify the webmaster to remove it
// IMMEDIATELY!
// 
// Thank you for your cooperation!
// 
// 
// **********************************************
// dev/Foes/Foe Compiler/Program.cs
// (c) 2021 Jeroen Petrus Broks
// Version: 21.11.10
// EndLic
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrickyUnits;

namespace Foe_Compiler {
	class Program {

		const string _Source = "Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale REVAMPED/dev/Foes";
		const string _Target = "Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale REVAMPED/src/Tricky Script/Script/Data/Foe/";
		static string Source => Dirry.AD(_Source);
		static string Target => Dirry.AD(_Target);
		static int Errors = 0;
		static StringBuilder Bestiary = new StringBuilder((@"
#use 'Libs/LinkedList'

Class Monster
	Var _Data
	ReadOnly String File
	ReadOnly String Name
	Get Var Data
		_Data = Data || NeilUse('Script/Data/Foe/'..File)
		Return _Data
	End

	Static Var Lijst = new TLinkedList
	Constructor(String F, String N)
		File = F
		Name = N
	End
End

Init

").Replace("'","\""));

		static void Fout(string F) {
			QCol.QuickError(F);
			Console.Beep();
			Errors++;
		}

		static void Compile(string F) {
			try {
				QCol.Doing("Compiling", F);
				var source = QuickStream.LoadString($"{Source}/{F}").Replace("\r", "");
				var Lines = source.Split('\n');
				var Data = GINI.ReadFromLines(Lines);
				var OutBool = new SortedDictionary<string, bool>();
				var OutInt = new SortedDictionary<string, int>();
				var OutString = new SortedDictionary<string, string>();				
				var Output = new StringBuilder($"// Generated {DateTime.Now}\n// (c) Jeroen P. Broks\n// May NOT be extracted from this game without prior written permission from Jeroen P. Broks!\n\n\nTable _R(Bool Oversoul)\n\tTable Ret = {"{"}{"}"}\n"); Output.Append("\tRet.Drops = {}\n\tRet.Steal = {}\n\n");
				var Drops = new Dictionary<string, int>();
				var Rates = new SortedDictionary<string, int>();
				bool AllowOversoul() => OutBool["OVERSOUL"];
				var DropCount = 0;
				var StealCount = 0;
				Bestiary.Append($"\tMonster.Lijst.AddLast(New Monster(\"{F}\",\"{Data["Name"]}\"))\n");
				foreach (var k in Data.Vars()) {
					var ks = k.Trim().Split(' ');
					var keytype = ks[0];
					var key = ""; if (ks.Length > 1) key = ks[1];
					switch (keytype) {
						case "BOOL":
							OutBool[key] = qstr.ToInt(Data[k]) > 0;
							if (key == "OVERSOUL" && (!AllowOversoul())) Output.Append($"\tIf Oversoul\n\t\tLua.error(\"Enemy '{F}' does not support oversoul!\")\n\tEnd\n\n");
							if (key == "BOSS") Output.Append($"\tRet.Boss = {OutBool[key].ToString().ToLower()}\n");
							break;
						case "NUM":
							OutInt[key] = qstr.ToInt(Data[k]);
							if (key == "CASH" || qstr.Prefixed(key, "SR_")) Output.Append($"\tRet.{key} = {OutInt[key]}\n");
							if (qstr.Prefixed(key, "ER_")) {
								OutInt["OVERSOUL_"+key] = qstr.ToInt(Data[k]);
								OutInt["NORMAL_" + key] = qstr.ToInt(Data[k]);
							}
								break;
						case "ITEM":
							var rate = qstr.ToInt(Data[$"NUM {k.Replace("ITEM", "RATE")}"]);
							if (rate>500) { QCol.Red("WARNING! "); QCol.Yellow($"Item rate {rate} for {k} is too high! Max is 500!\n "); rate = 500; Console.Beep(140, 20); }
							//QCol.Doing($"NUM {k.Replace("ITEM","RATE")}", Data[$"NUM {k.Replace("ITEM","RATE")}"]); // debug
							switch (key) {
								case "DROP":
									//for (int i = 0; i < rate; i++) Output.Append($"\tRet.Drops += \"{Data[k]}\"\n");
									Output.Append($"\tRet.DropRate_{++DropCount} = {rate}\n\tRet.DropItem_{DropCount} = \"{Data[k]}\"\n");
									break;
								case "STEAL":
									//for (int i = 0; i < rate; i++) Output.Append($"\tRet.Steal += \"{Data[k]}\"\n");
									Output.Append($"\tRet.StealRate_{++StealCount} = {rate}\n\tRet.StealItem_{StealCount} = \"{Data[k]}\"\n");
									break;
								default:
									Fout($"Unknown Item direction {key}");
									break;
							}														
							break;
						case "RATE":
							Rates[key.Replace("RATE_", "")] = qstr.ToInt(Data[k]);
							break;
						default:
							if (k.Contains(" ")) Fout($"Invalid key {k}");
							else {
								OutString[k] = Data[k];
								if (!qstr.Prefixed(k, "TARGET_")) Output.Append($"\tRet.{k} = \"{Data[k].Replace("\n", "\\n").Replace("\"","\\\"")}\"\n");
							}
							//Fout($"Unknown data group {key} (fk: {k})");
							break;
					}
				}
				// Stats Normal
				Output.Append("\n\n\tRet.Stats = {}\n");
				Output.Append("\n\n\tIf !Oversoul\n");
				foreach(var s in OutInt) {
					if (qstr.Prefixed(s.Key, "NORMAL_")) Output.Append($"\t\tRet.Stats.{s.Key.Replace("NORMAL_","")} = {s.Value}\n");
				}
				// Stats oversoul
				if (AllowOversoul()) {
					Output.Append("\tElse\n");
					foreach (var s in OutInt) {
						if (qstr.Prefixed(s.Key, "OVERSOUL_")) Output.Append($"\t\tRet.Stats.{s.Key.Replace("OVERSOUL_", "")} = {s.Value}\n");
					}
				}
					Output.Append("\tEnd\n\n");
				// Actions
				Output.Append("\tRet.Actions = {}\n");
				bool QOutBool(string f) { if (!OutBool.ContainsKey(f)) { QCol.Red("WARNING! "); QCol.Yellow($"No Outbool Key '{f}'\n"); return false; } else return OutBool[f]; }
				foreach(var rate in Rates) {
					string rk = rate.Key;
					string boolean = "True";
					if (rk == rk.Replace("ACTION_", "")) {
						if (AllowOversoul() && (!QOutBool($"OVERSOUL_{rk}"))) boolean += " && (!Oversoul)";
						//if (AllowOversoul() && (QOutBool($"NORMAL_{rk}"))) boolean += " && (Oversoul)";
						if (AllowOversoul() && (!QOutBool($"NORMAL_{rk}")) && QOutBool($"OVERSOUL_{rk}")) boolean += " && (Oversoul)";
						if (!QOutBool($"NEWGAMEPLUS_{rk}")) boolean += " && (!__NewGamePlus)";
						for (int i = 1; i <= 3; ++i) {
							if (!QOutBool($"SKILL{i}_{rk}")) boolean += $" && (__skill!={i})";
						}
						Output.Append($"\tIf {boolean}\n");
						for (int i = 0; i < rate.Value; ++i) Output.Append($"\t\tRet.Actions += \"{rk}\"\n");
						Output.Append("\tEnd\n");
					}
				}
				// Save
				Output.Append("\tReturn Ret\nEnd\nReturn _R\n\n");
				var OutFile = $"{Target}/{F}.neil";
				Directory.CreateDirectory(qstr.ExtractDir(OutFile));
				QuickStream.SaveString(OutFile, Output);
			} catch(Exception E) {
				Fout($".NET error thrown: \"{E.Message}\"");
				QCol.Magenta($"{E.StackTrace}\n");
			}
		}
		

		static void Main(string[] args) {
			Dirry.InitAltDrives();
			QCol.DoingTab = 15;
			QCol.Doing("Analysing", _Source);
			var Lijst = FileList.GetTree(Source);
			foreach(var F in Lijst) {
				if (!qstr.Prefixed(F.ToUpper(), "FOE COMPILER/")) Compile(F);
			}
			QCol.Doing("Saving", "General bestiary data"); // This will make the bestiary loader faster. The loading time is already long enough the way it is.
			Bestiary.Append($"End\n\n// This script was last generated {DateTime.Now} //\n\n");
			QuickStream.SaveString(Dirry.AD("Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/src/Tricky Script/Script/Data/General/Bestiary.neil"),Bestiary);
			QCol.Doing("Errors", $"{Errors}");
			TrickyDebug.AttachWait();
		}
	}
}