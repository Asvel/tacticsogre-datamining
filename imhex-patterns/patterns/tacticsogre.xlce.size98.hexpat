#pragma MIME application/x-tacticsogre-xlce-size98
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

struct XlceEntry_BattleData11_Equipment {
	padding[2];
	u8 equipmentType;
	padding[10];
	bool isTwoHand;
	padding[4];
	u8 attackTypeId;
	padding[20];
	u8 weaponPhysicalType;  // CRUSHING, SLASHING, PIERCING
	u8 weaponPhysicalValue;
	padding[16];
	u8 weaponElementType;
	u8 weaponElementValue;
	padding[75];
	u16 nameId;
	padding[16];
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
