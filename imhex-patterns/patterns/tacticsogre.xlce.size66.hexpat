#pragma MIME application/x-tacticsogre-xlce-size66
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

struct XlceEntry_BattleData25_Skill {
	padding[2];
	u8 skillType;  // 5=auto
	padding[12];
	u8 triggerChance;  // x%
	padding[74];
	u16 nameId;
	u16 helpId;
	padding[8];
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
