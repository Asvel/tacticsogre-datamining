#pragma MIME application/x-tacticsogre-xlce-sizeD0
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

struct XlceEntry_BattleData14_Class {
	padding[4];
	u16 growthHp;
	padding[4];
	u16 growthMp;
	padding[4];
	u16 growthStr;
	padding[4];
	u16 growthVit;
	padding[4];
	u16 growthDex;
	padding[4];
	u16 growthAgi;
	padding[4];
	u16 growthAvd;
	padding[4];
	u16 growthInt;
	padding[4];
	u16 growthMnd;
	padding[4];
	u16 growthRes;
	padding[2];
	u16 growthRandom;
	padding[144];
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
