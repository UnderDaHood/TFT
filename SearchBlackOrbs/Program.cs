// Lic:
// Black Orb Counter
// Debug
// 
// 
// 
// (c) Jeroen P. Broks, 2022
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 22.04.30
// EndLic

// See https://aka.ms/new-console-template for more information
using System.Text;
using TrickyUnits;
using UseJCR6;
using NSKthura;

const string KthDir = "E:/Projects/Applications/Apollo/Games/The Fairy Tale Revamped/src/Tricky Private/Kthura/";
const string OutputFile = "E:/Projects/Applications/Apollo/Games/The Fairy Tale Revamped/src/Tricky Script/Script/Data/General/BlackOrbsDebug.neil";
int OrbsCounted = 0;
var OrbsPerMap = new SortedDictionary<string, int>();
var OrbsPerLayer = new SortedDictionary<string, SortedDictionary<string, int>>();

void Count(string m) {
	QCol.Doing("Analysing", m);
	m.ToUpper();
	OrbsPerMap[m] = 0;
	OrbsPerLayer[m] = new SortedDictionary<string, int>();
	//          123456789
	var km = Kthura.Load($"{KthDir}{m}");
	foreach (var l in km.Layers) {
		QCol.Doing("= Layer", l.Key);
		OrbsPerLayer[m][l.Key] = 0;
		foreach (var o in l.Value.Objects) {			
			if (qstr.Prefixed(o.Tag, "BLACKORB_")) {
				QCol.Magenta(".");
				OrbsCounted++;
				OrbsPerMap[m]++;
				OrbsPerLayer[m][l.Key]++;
			}
		}
		Console.Write("\r");
		if (OrbsPerLayer[m][l.Key] > 0) QCol.Doing("InLay", $"{OrbsPerLayer[m][l.Key]}");
	}
	QCol.Doing("= InMap", $"{OrbsPerMap[m]}");
	QCol.Doing("= Total", $"{OrbsCounted}");
}

JCR6_zlib.Init();
//JCR6_lzma.Init();
new KthuraDrawFake();

MKL.Version("The Fairy Tale REVAMPED for Apollo - Program.cs","22.04.30");
MKL.Lic    ("The Fairy Tale REVAMPED for Apollo - Program.cs","GNU General Public License 3");

QCol.Magenta("Search for Black Orbs\n");
QCol.Doing("Version", MKL.Newest);
QCol.Doing("Coded by", "Jeroen P. Broks");
Console.WriteLine();
QCol.Doing("Analysing",KthDir);
var d = FileList.GetDir(KthDir);
foreach(var mp in d) Count(mp);
QCol.Doing("Total", $"{OrbsCounted}");
Console.WriteLine();
QCol.Doing("Generating", "Output");

var script = new StringBuilder("// BlackOrbs\n\nVar Ret = { [\"IGNORE\"] = {} }\n\nInit\n");
var NW = "{}";
var first = true;
foreach (var m in OrbsPerLayer) {
	if (!first) script.Append("\n"); first = false;
	script.Append($"\tRet[\"{m.Key}\"] = {NW}\n");
	foreach (var l in m.Value) {
		script.Append($"\tRet[\"{m.Key}\"][\"{l.Key}\"] = {l.Value}\n");
	}
}
script.Append("End\n\nReturn Ret\n");
QCol.Doing("Saving", OutputFile);
QuickStream.SaveString(OutputFile, script);

QCol.Cyan("Ok");
Console.ResetColor();