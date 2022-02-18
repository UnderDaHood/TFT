Const W = 255
Const H = 360


AppTitle "Big sinus"

; Up to down
Graphics W*4,H

For y=0 To H-1
	CX = (W*2)+(Sin(y) * W)
	For x=0 To 255
		Color 255-x,255-x,255-x
		Plot CX+X,Y
		Plot CX-X,Y
	Next
Next
img = CreateImage(W*4,H)
GrabImage img,0,0
SaveImage img,"VSinus.bmp"

WaitKey


; Left to right
Graphics H,W*4

For y=0 To H-1
	CX = (W*2)+(Sin(y) * W)
	For x=0 To 255
		Color 255-x,255-x,255-x
		Plot Y,CX+X
		Plot Y,CX-X
	Next
Next
img = CreateImage(H,W*4)
GrabImage img,0,0
SaveImage img,"HSinus.bmp"

WaitKey
End

