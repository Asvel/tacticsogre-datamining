#pragma MIME application/x-tacticsogre-xlce-size54
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

struct XlceEntry_BattleData23_Race {
	padding[2];
	u8 gender;
	padding[25];
	u8 growthHp;
	u8 growthMp;
	padding[1];
	u8 growthStr;
	padding[1];
	u8 growthVit;
	padding[1];
	u8 growthDex;
	padding[1];
	u8 growthAgi;
	padding[1];
	u8 growthAvd;
	padding[1];
	u8 growthInt;
	padding[1];
	u8 growthMnd;
	padding[1];
	u8 growthRes;
	padding[10];
	u8 classChangeType;
	u8 classChangeRace;
	padding[26];
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
