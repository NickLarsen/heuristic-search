<Query Kind="Statements">
  <NuGetReference>StacMan</NuGetReference>
</Query>

var state = new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
var stateString = string.Join(", ", state);
var moveSequences = new string[]{
	"llluuu",
	"lluluu",
	"lluulu",
	"lluuul",
	"lulluu",
	"lululu",
	"luluul",
	"luullu",
	"luulul",
	"luuull",
};
string outputFormat = "new Symmetry(\"MD\", new byte[] {{ {0} }}, new byte[] {{ {1} }}, 6u),";
foreach(var s in moveSequences)
{
	var transformed = state.ToArray();
	var blank = 15;
	foreach(var m in s)
	{
		int c = blank - (m == 'l' ? 4 : 1);
		var temp = transformed[blank];
		transformed[blank] = transformed[c];
		transformed[c] = temp;
		blank = c;
	}
	var transOutput = string.Join(", ", transformed);
	string.Format(outputFormat, stateString, transOutput).Dump();
	
	var transformed2 = state.ToArray();
	blank = 15;
	foreach(var m in s)
	{
		int c = blank - (m == 'l' ? 1 : 4);
		var temp = transformed2[blank];
		transformed2[blank] = transformed2[c];
		transformed2[c] = temp;
		blank = c;
	}
	var transOutput2 = string.Join(", ", transformed2);
	string.Format(outputFormat, stateString, transOutput2).Dump();
}