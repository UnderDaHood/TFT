// Lic:
// **********************************************
// 
// dev/CreditsData/Program.cs
// (c) Jeroen Broks, 2022, All Rights Reserved.
// 
// This file contains material that is related
// to a storyline that is which is strictly
// copyrighted to Jeroen Broks.
// 
// This file may only be used in an unmodified
// form with an unmodified version of the
// software this file belongs to.
// 
// You may use this file for your study to see
// how I solved certain things in the creation
// of this project to see if you find valuable
// leads for the creation of your own.
// 
// Mostly this file comes along with a project
// that is for most part released under an
// open source license and that means that if
// you use that code with this file removed
// from it, you can use it under that license.
// Please check out the other files to find out
// which license applies.
// This file comes 'as-is' and in no possible
// way the author can be held responsible of
// any form of damages that may occur due to
// the usage of this file
// 
// 
// **********************************************
// 
// version: 22.02.14
// EndLic

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrickyUnits;
using UseJCR6;

namespace CreditsData {
	class MainProgram {

		static GINIE Data;
		static TJCRDIR Res;

		static List<string> MainTeam = new List<string>();
		static List<string> CC = new List<string>();
		static Dictionary<string, List<string>> Contributions = new Dictionary<string, List<string>>();

		static string DataFile => Dirry.AD("Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/dev/CreditsData/CreditsData.ini");
		static string ResFile => Dirry.AD("Scyndi:Projects/Applications/Apollo/Games/The Fairy Tale Revamped/Debug/The Fairy Tale REVAMPED.jcr");

		static bool Yes(string c,string v,string question) {
			while (Data[c, v] == "") {
				QCol.Yellow($"{question} ");
				QCol.Cyan("? ");
				QCol.Magenta("(Y/N) ");
				var a = Console.ReadKey(true);
				switch (a.Key) {
					case ConsoleKey.Y:
						Data[c, v] = "YES";
						QCol.Green("Yes\n");
						break;
					case ConsoleKey.N:
						Data[c, v] = "NO";
						QCol.Red("No\n");
						break;
					default:
						QCol.White("???\n");
						break;
				}
			}
			return Data[c, v].ToUpper() == "YES";
		}

		static string Ask(string c,string v,string question,string defaultanswer = "") {
			while (Data[c, v] == "") {
				if (defaultanswer != "") QCol.Magenta($"[{defaultanswer}] ");
				QCol.Yellow(question);
				QCol.Cyan(" ");
				Data[c, v] = Console.ReadLine().Trim();
			}
			return Data[c, v];
		}

		static void AddContributor(StringBuilder o,string contributor) {
			try {
				bool got = false;
				var Before = new List<string>();
				Contributions[contributor].Sort();
				Debug.WriteLine($"{o}/{contributor}/{got},{Before}");
				foreach(var s in Contributions[contributor]) {
					if (Yes("Credit_Add",$"{contributor}.{s}",$"Credit {contributor} of {s}") && (!Before.Contains(s))) {
						if (!got) o.Append($"\tACL(\"\",0,0,0)\n\tACL(\"{contributor}\",255,180,0)\n"); got = true;
						o.Append($"\tACL(\"{s}\",255,255,255,0)\n");
						Before.Add(s);
					}
				}
			} catch(Exception E) {
				QCol.QuickError(E.Message);
			}
		}

		static void ACL(StringBuilder o,string txt,byte R=255,byte G=255,byte B=255,uint white = 0) {
			o.Append($"\tACL(\"{txt}\", {R}, {G}, {B}, {white})\n");
		}

		static void Voices(StringBuilder o) {
			o.Append("\tACL(\"\",0,0,0,100)\n\tACL(\"Cast\",255,180,0)\n");
			foreach(string ch in Data.List("VOICES", "ORDER")) {
				o.Append($"\tACL(\"{ch}: {Ask("VOICES", $"ACTOR::{ch}", $"{ch} is voiced by: ")}\",255,255,255)\n");
			}
		}

		static void White(StringBuilder o, uint ExtraWhite = 0) => ACL(o, "", 0, 0, 0, ExtraWhite);

		static void Main(string[] args) {
			Dirry.InitAltDrives();
			QCol.Doing("Reading", DataFile);
			Data = GINIE.FromFile(DataFile);
			Data.AutoSaveSource = DataFile;
			QCol.Doing("Reading", ResFile);
			Res = JCR6.Dir(ResFile);
			foreach(var ent in Res.Entries.Values) {
				var D = qstr.ExtractDir(ent.Entry);
				var E = qstr.ExtractExt(ent.Entry).ToUpper();
				var A = ent.Author;
				string C = "";
				if (Yes("MainTeam", A, $"Is author {A} a member of the main team") && (!MainTeam.Contains(A)))
					MainTeam.Add(A);
				else if (!CC.Contains(A))
					CC.Add(A);
				if (Yes("Dir_Is_Cat",D,$"Is directory '{D}' (where {ent.Entry} is) a category on its own")) {
					C = Ask("Dir_Cat", D, $"And what category is {D} then? ");
				} else if (E!="") {
					C = Ask("Ext_Cat", E, $"File {ent.Entry} has extension '{E}'. What category is that? ");
				}
				if (!Contributions.ContainsKey(A)) Contributions[A] = new List<string>();
				if (!Contributions[A].Contains(C)) {
					QCol.Doing(A, C);
					Contributions[A].Add(C);
				}				
				
			}

			var output = new StringBuilder((@"
Class CreditLine
	ReadOnly String Text
	ReadOnly Int R
	ReadOnly Int G
	ReadOnly Int B
	ReadOnly Var Img
	Int Y
	ReadOnly Int White
	ReadOnly Int BaseY
	Static Int AllBaseY = 0

	Static Var Reg = new TLinkedList()

	Constructor(String _Text,Int _R, Int _G, Int _B,Int _White=0)
		R = _R
		G = _G
		B = _B
		Text = _Text
		If Prefixed(Text,''IMG:'')
			Img = Image.Load(Right(Text,#Text-4))
			Img.HotBottomCenter()
			AllBaseY += Img.Height
			BaseY = AllBaseY
		Else
			BaseY = AllBaseY
			AllBaseY += 30
		End
		White = _White
		AllBaseY += _White
	End
End

Void ACL(String L,Int _R, Int _G, Int _B,Int White=0)
  CreditLine.Reg.AddLast(New CreditLine(L,_R,_G,_B,White))
End

Init
	ACL(''IMG:GFX/Logo/Title.png'',255,255,255)
	ACL(''A game by Jeroen P. Broks'',255,180,0,200)
").Replace("''","\""));

			// Main contributor
			var MC = Contributions[Data["Main", "MainContributor"]];
			MC.Add("Apollo Game Engine");
			MC.Add("Jeroen's Collected Resource version 6 (JCR6)");
			MC.Add("Kthura Map System");
			MC.Add("Neil Scripting Language");
			AddContributor(output, Ask("Main", "MainContributor", "Main Contributor:"));
			Voices(output);
			White(output,100);
			ACL(output, "Extra assistance from", 0, 180, 255, 20);
			foreach (var c in MainTeam) if (c != Data["Main", "MainContributor"]) AddContributor(output, c);


			void AddCC(string cnt,params string[] c) {
				CC.Add(cnt);
				if (!Contributions.ContainsKey(cnt)) Contributions[cnt] = new List<string>();
				foreach (var ic in c)
					Contributions[cnt].Add(ic);
			}
			AddCC("Sam Lantinga & the SDL Team", "Simple Direct Media Layer (SDL)");
			AddCC("PUC RIO", "Lua scripting engine");
			AddCC("Jean-loup Gailly and Mark Adler", "zlib compression library");

			White(output,200);
			ACL(output, "And then I wish to thank the following", 0, 180, 255);
			ACL(output, "contributors to the free community.", 0, 180, 255);
			ACL(output, "Without their selfless efforts", 0, 180, 255);
			ACL(output, "this game would not have been posisble", 0, 180, 255);

			CC.Sort();
			foreach (var c in CC) if (!MainTeam.Contains(c)) AddContributor(output, c);

			White(output, 200);
			ACL(output, "The entire story is fictional.");
			ACL(output, "Any resemblance to real people, either famous or unknown,");
			ACL(output, "either dead or alive, is merely coincidental");
			White(output, 200);
			ACL(output, "(c) 2004, 2017, 2022 Jeroen P. Broks");

			// Always last!
			output.Append("End");
			QuickStream.SaveString(Dirry.AD(Ask("Output","Script","Output Neil Script:")),output);
			TrickyDebug.AttachWait();
		}
	}
}