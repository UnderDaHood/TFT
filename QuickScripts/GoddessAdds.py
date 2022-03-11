"""   -- Start License block
**********************************************

dev/QuickScripts/GoddessAdds.py
(c) Jeroen Broks, 2022, All Rights Reserved.

This file contains material that is related
to a storyline that is which is strictly
copyrighted to Jeroen Broks.

This file may only be used in an unmodified
form with an unmodified version of the
software this file belongs to.

You may use this file for your study to see
how I solved certain things in the creation
of this project to see if you find valuable
leads for the creation of your own.

Mostly this file comes along with a project
that is for most part released under an
open source license and that means that if
you use that code with this file removed
from it, you can use it under that license.
Please check out the other files to find out
which license applies.
This file comes 'as-is' and in no possible
way the author can be held responsible of
any form of damages that may occur due to
the usage of this file


**********************************************

version: 22.03.11
-- End License block   """
import datetime

Adds = {
	"BlockGun":"Blocking Gun",
	#"ConfuseGun",
	"DarkGun":"Dark Gun",
	"FlameThrower":"Flame Thrower",
	"Gunner":"Gunner",
	"IceCannon":"Ice Cannon",
	"LightGun":"Light Gun",
	"RockThrower":"Rock Thrower",
	"TRQ":"Tranqualizer Gun",
	"VenomGun":"Venom Gun",
	"VirusGun":"Virus Gun",
	"WaterGun":"Water Gun",
	"WindGun":"Wind Gun"
}


Basis = """
AI=default
BOOL BOSS=1
ITEM DROP 1=ITM_X_AURINA
ITEM DROP 2=ITM_X_BANDAGE
ITEM STEAL 1=ITM_X_AURINA
ITEM STEAL 2=ITM_X_BANDAGE
ITEM STEAL 3=ITM_X_ENERGYDRINK
NUM CASH=0
NUM ER_DARKNESS=0
NUM ER_EARTH=00
NUM ER_FLAME=00
NUM ER_FROST=00
NUM ER_LIGHT=00
NUM ER_LIGHTNING=10000
NUM ER_WATER=00
NUM ER_WIND=0
NUM NORMAL_ACCURACY=100
NUM NORMAL_ENDURANCE=455
NUM NORMAL_EVASION=5
NUM NORMAL_HP=65000
NUM NORMAL_INTELLIGENCE=100
NUM NORMAL_POWER=650
NUM NORMAL_RESISTANCE=50
NUM NORMAL_SPEED=350
NUM OVERSOUL_ACCURACY=100
NUM RATE DROP 1=100
NUM RATE DROP 2=25
NUM RATE STEAL 1=75
NUM RATE STEAL 1=50
NUM RATE STEAL 1=25
NUM SR_CURSE =100
NUM SR_DEATH=100
NUM SR_DISEASE =100
NUM SR_PARALYSIS =100
NUM SR_PETRIFICATION =100
NUM SR_POISON =100
NUM SR_SILENCE =100
NUM SR_UNDEAD =100
NUM NORMAL_EXP=0
"""

Outpath="E:/Projects/Applications/Apollo/Games/The Fairy Tale Revamped/Dev/Foes/Goddess/"
#Outpath="E:\Projects\Applications\Apollo\Games\The Fairy Tale Revamped\dev\Foes\Goddess\"

def Save(a):
	n=Adds[a]
	print("Add: "+a)
	d = datetime.datetime.now()
	with open(Outpath+a, "w") as text_file:
		text_file.write("[rem]\nGenerated with GoddessAdds.py\nCopyright Jeroen P. Broks\nOnly available for evaluation purposes\n\n")
		text_file.write("Generated: %02d/%02d/%04d\n\n"%(d.day,d.month,d.year))
		text_file.write("[vars]\n%s\n"%Basis)
		text_file.write("Name=%s\nDesc=Automated gun assisting the Goddess\n"%n)
		text_file.write("BOOL NEWGAMEPLUS_ABL_FOE_X_G_%s=1\n"%a)
		text_file.write("BOOL NORMAL_ABL_FOE_X_G_%s=1\n"%a)
		text_file.write("BOOL OVERSOUL_ABL_FOE_X_G_%s=1\n"%a)
		text_file.write("BOOL SKILL1_ABL_FOE_X_G_%s=1\n"%a)
		text_file.write("BOOL SKILL2_ABL_FOE_X_G_%s=1\n"%a)
		text_file.write("BOOL SKILL3_ABL_FOE_X_G_%s=1\n"%a)
		text_file.write("RATE RATE_ABL_FOE_X_G_%s=2\n"%a)
		text_file.write("Image=GFX/Combat/Fighters/Foe/Goddess/Add_%s.png"%a)
		
for a in Adds:
	Save(a)
print("That should do it!")