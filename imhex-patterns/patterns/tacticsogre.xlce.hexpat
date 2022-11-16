#pragma MIME application/x-tacticsogre-xlce
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
};

struct XlceEntry_Common32 {
	u32 data[parent.entrySize / 4] [[inline]];
};

struct XlceEntry_Common64 {
	u32 data[parent.entrySize / 8] [[inline]];
};

bitfield XlceEntry__GlobalFlagStorage {
    sizeType : 4;
    if (sizeType == 1) {
        bitPosition: 3;
        bytePosition: 9;
    }
    else if (sizeType == 2) {
        bitPosition: 1;
        bytePosition: 11;
    }
    else if (sizeType == 3) {
        bytePosition: 12;
    }
    else if (sizeType == 4) {
        bytePosition: 12;
    }
    else {
        bytePosition: 12;
    }
};
struct XlceEntry_GlobalFlag {
	XlceEntry__GlobalFlagStorage position [[inline]];
	u16 groupMask;
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

struct XlceEntry_BattleData23_Race {
	padding[28];
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
	padding[38];
};

using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
