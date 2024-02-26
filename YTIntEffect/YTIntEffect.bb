AppTitle "YouTube Intro Effect"
Graphics 800,600,32,0

;Global JM% = LoadImage("E:\Projects\Applications\Apollo\Games\The Fairy Tale Revamped\src\Aeris\GFX\Big_Char\JakeMarrilona.png")
Global JM% = LoadImage("JakeMarrilona.png")
Global JMX% = 800
Global CN% = LoadImage("E:\Projects\Applications\Apollo\Games\The Fairy Tale Revamped\src\Tricky CC\GFX\System\Console.png")
Global Deg%
Global TT% = LoadImage("E:\Projects\Applications\Apollo\Games\The Fairy Tale Revamped\src\Tricky Private\GFX\Logo\Title.png")
HandleImage TT,ImageWidth(TT)/2,ImageHeight(TT)/2
MaskImage JM,255,0,255
Global TY = 600+ImageHeight(TT)
SetBuffer BackBuffer()
WaitKey
Repeat
	Deg = (Deg + 1) Mod 360
	TileImage CN,0,Sin(Deg)*75
	If JMX>800-435 Then JMX = JMX -1
	DrawImage JM,JMX,0
	If TY>300 Then TY = TY -1
	DrawImage TT,400,TY
	Flip
Until KeyHit(1)
FreeImage JM
FreeImage CN
FreeImage TT
End