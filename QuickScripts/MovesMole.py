Moves = {
	"AAA_ATTACK":10,
	"AAA_GUARD":1,
	"ABL_FOE_X_STUNGUN":3,
	"ABL_FOE_X_M_DART":5,
	"ABL_FOE_X_M_HEALSPRAY":7,
	"ABL_FOE_X_M_NAPALM":1,
	"ITM_HEALSHOWER":2,
	"ABL_FOE_X_M_RISINGNOVA":4,
	"ABL_FOE_X_M_FLAMETHROWER":8,
	"ABL_FOE_X_M_ICEBULLET":8
}


Prefixes =("Rate RATE_","Target_","bool NORMAL_","bool NEWGAMEPLUS_","bool OVERSOUL_","bool SKILL1_","bool SKILL2_","bool SKILL3_")


for M in Moves:
	for P in Prefixes:
		if P=="Rate RATE_":
			print("%s%s=%4d"%(P,M,Moves[M]))
		else:
			print("%s%s=1"%(P,M))
			