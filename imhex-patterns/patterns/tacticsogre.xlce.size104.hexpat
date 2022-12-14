#pragma MIME application/x-tacticsogre-xlce-size104
#pragma endian little

using XlceEntry;

struct Xlce {
	char signature[4];
	u32 count;
	u32 bodyStart;
	u32 entrySize;
	XlceEntry entries[count] @ bodyStart [[inline]];
};

struct XlceEntry_Common {
	u8 data[parent.entrySize] [[inline]];
	// u32 data[parent.entrySize / 4] [[inline]];
};

struct XlceEntry_BattleData19_AttackDefenseType {
	// BattleData19[attackTypeId] is offense factors
	// BattleData19[attackTypeId + 1] is defense factors
	s16 offenseEquipmentAtkFactor;
	s16 defenseEquipmentDefFactor;
	s16 selfToActionElementFactors[3];
	s16 actionAtkFactor;
	s16 statFactors[6];  // STR, VIT, DEX, INT, MND, RES
	padding[46];
	s16 defenseStepFactor;  // STEP, aka. height
	s16 defenseInverseStepFactor;  // STEP, aka. height
	padding[130];
	s16 actionElementFactor[8];  // index is element type
	padding[16];
	s16 selfToTargetElementFactors[3];
	padding[18];
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
